using Newtonsoft.Json;

namespace Comunica.ProcessManager.Web.Code;

public class ReportingDocumentWeb : IReportingDocumentWeb
{
    private readonly ILogger<ReportingDocumentWeb> _logger;
    private readonly ContextApp _contextApp;

    private readonly IReportConfigLogic _logicReportConfig;

    private readonly MainLocalizer _localizer;
    private readonly IReportingDataTableWeb _webReportingDataTable;
    private readonly IReportingMapperWeb _webReportingMapper;

    public ReportingDocumentWeb(
        ILogger<ReportingDocumentWeb> logger
        , ContextApp contextApp
        , IReportConfigLogic logicReportConfig
        , MainLocalizer localizer
        , IReportingDataTableWeb webReportingDataTable
        , IReportingMapperWeb webReportingMapper
        )
    {
        _logger = logger;
        _contextApp = contextApp;
        _logicReportConfig = logicReportConfig;
        _localizer = localizer;
        _webReportingDataTable = webReportingDataTable;
        _webReportingMapper = webReportingMapper;
    }


    /// <summary>
    /// create excel from items information retrieved. Filters will be included in document
    /// </summary>
    /// <param name="reportDataDumpModel"></param>
    /// <param name="getForAllAvailableProcesses"></param>
    /// <param name="dateSubmitFrom"></param>
    /// <param name="dateSubmitTo"></param>
    /// <returns></returns>
    public FileDownloadInfoModel BuildExcelReport(
        ReportExportDataModel reportDataDumpModel
        , bool getForAllAvailableProcesses
        , string dateSubmitFrom
        , string dateSubmitTo
        , bool hasItemEndEditableField
        , string dateExpirationFromFilter
        , string dateExpirationToFilter
        )
    {
        DateTime timeStamp = DateTime.Now;

        //at this moment we can't configure from db HOW to retrieve data for data areas
        //so for now we only associate data to area config by index
        IList<ReportingAreaModel> dataToLoad =
            new List<ReportingAreaModel>
            {
                new ReportingAreaModel
                {
                    Type = ReportAreaDataToLoad.DocumentInfo,
                    Data =
                        _webReportingDataTable.BuildFileInfoTable(
                            _contextApp.GetCurrentCulture()
                            , timeStamp
                            ),
                    StepIndex = 0,
                },

                new ReportingAreaModel
                {
                    Type = ReportAreaDataToLoad.FiltersUsed,
                    Data = _webReportingDataTable.GetDataTableFiltersUsed(
                                reportDataDumpModel
                                , getForAllAvailableProcesses
                                , dateSubmitFromStr: dateSubmitFrom
                                , dateSubmitToStr: dateSubmitTo
                                , hasItemEndEditableField: hasItemEndEditableField
                                , dateExpirationFromStr: dateExpirationFromFilter
                                , dateExpirationToStr: dateExpirationToFilter
                                ),
                    StepIndex = 0,
                }
            };

        bool mergeItemsPrimaryDataInOneArea =
            CheckIfAllItemsAreMergeableInOneArea(
               reportDataDumpModel
                );

        (
            IList<ReportingAreaModel> primaryAreaModelList
            , IList<ReportPositionGridForMasterProcessModel> gridsByProcessList
            ) = GetPrimaryAreasAndGrids(
                    reportDataDumpModel.ItemsBasicDataList
                    , reportDataDumpModel.ReportLinkedItemPrimaryInfoList
                    , mergeItemsPrimaryDataInOneArea
                    , hasItemEndEditableField
                    );
        _logger.LogAppInformation(
            "Serialized calculated report items positioning grids "
            + JsonConvert.SerializeObject(gridsByProcessList)
            );

        dataToLoad =
            dataToLoad.Concat(primaryAreaModelList)
                      .ToList();

        dataToLoad =
            GetItemsStepsData(
                dataToLoadAndReturn: dataToLoad
                , gridsByProcessList
                , reportDataDumpModel.ItemsFormsByProcessList
                , stepsAreaType: ReportAreaDataToLoad.DataStepItemsByProcess
                );
        dataToLoad =
            GetItemsStepsData(
                dataToLoadAndReturn: dataToLoad
                , gridsByProcessList
                , reportDataDumpModel.ItemLinkedFormDataByProcess
                , stepsAreaType: ReportAreaDataToLoad.DataStepLinkedItemsByProcess
                );


        IList<ReportingProcessStepsLgc> stepsCountByProcess =
            GetStepsCountByProcess(
                dataToLoad
                , mergeItemsPrimaryDataInOneArea
                );

        ReportConfigFileLgc reportConfigFile =
            _logicReportConfig.GetForAllItems(
                stepsCountByProcess
                , defaultSheetLocalizedName:
                    _localizer[nameof(LocalizedStr.ReportDumpItemsSheetName)]
                );

        FileDownloadInfoModel fileDownloadInfoModel =
            _webReportingMapper.MapFile(
                dataToLoad
                , reportConfigFile
                , timeStamp
                );

        return fileDownloadInfoModel;
    }



    #region private helper methods for BuildExcelReport

    /// <summary>
    /// allow all items in one area REGARDLESS of relative process
    /// only if linked items not present 
    /// and also steps data not present
    /// </summary>
    /// <param name="reportDataDumpModel"></param>
    /// <returns></returns>
    private static bool CheckIfAllItemsAreMergeableInOneArea(
        ReportExportDataModel reportDataDumpModel
        )
    {
        if (reportDataDumpModel.ReportLinkedItemPrimaryInfoList.HasValues()
            || reportDataDumpModel.ItemLinkedFormDataByProcess.HasValues())
        {
            return false;
        }

        if (reportDataDumpModel.ItemsFormsByProcessList.IsNullOrEmpty())
        {
            return true;
        }

        //items step forms data is always provided,
        //we need to check if at least one of the fields is present inside the object
        //if field is present data are not mergeable by process
        foreach (var pr in reportDataDumpModel.ItemsFormsByProcessList)
        {
            foreach (var form in pr.ItemsFormsByFormCodeList)
            {
                foreach (var itemStep in form.ItemFormDisplayBasicList)
                {
                    if (itemStep.SubmittedInputList.Count > 0)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private static IList<ReportingProcessStepsLgc> GetStepsCountByProcess(
        IList<ReportingAreaModel> reportingAreaModels
        , bool mergeItemsPrimaryDataInOneArea
        )
    {
        IList<ReportingProcessStepsLgc> reportingProcessSteps = new List<ReportingProcessStepsLgc>();

        foreach (long processId in reportingAreaModels
                                            .Select(ram => ram.ProcessId)
                                            .Distinct())
        {
            if (!mergeItemsPrimaryDataInOneArea
                && processId == 0)
            {
                //NECESSARY: this is a quirk of data areas building
                continue;
            }
            reportingProcessSteps.Add(
                new ReportingProcessStepsLgc
                {
                    ProcessId = processId,
                    Steps =
                        reportingAreaModels.Any(ram => ram.ProcessId == processId
                                                            && ram.Type == ReportAreaDataToLoad.DataStepItemsByProcess)
                        ? reportingAreaModels.Where(ram => ram.ProcessId == processId
                                                            && ram.Type == ReportAreaDataToLoad.DataStepItemsByProcess)
                                             .Select(ram => ram.StepIndex)
                                             .Max()
                        : 0,
                    StepsLinked =
                        reportingAreaModels.Any(ram => ram.ProcessId == processId
                                                            && ram.Type == ReportAreaDataToLoad.DataStepLinkedItemsByProcess)
                        ? reportingAreaModels.Where(ram => ram.ProcessId == processId
                                                            && ram.Type == ReportAreaDataToLoad.DataStepLinkedItemsByProcess)
                                             .Select(ram => ram.StepIndex)
                                             .Max()
                        : 0,
                }
                );
        }
        return reportingProcessSteps;
    }





    /// <summary>
    /// this list is needed to keep track of all items present in data dump.
    /// Necessary because when we will add data of relative steps
    /// not all items will have all steps completed so we will need to add a empty row.
    /// This class is added as transient so there is not risk using an external var.
    /// If data can't be merged, data areas must be kept separated by process
    /// </summary>
    private (
        IList<ReportingAreaModel> primaryAreaModelList
        , IList<ReportPositionGridForMasterProcessModel> gridsByProcessList
        )
        GetPrimaryAreasAndGrids(
            IList<ReportItemPrimaryInfoModel> reportItemPrimaryInfoList
            , IList<ReportItemPrimaryInfoModel> reportItemLinkedPrimaryInfoList
            , bool mergeItemsPrimaryDataInOneArea
            , bool hasItemEndEditableField
            )
    {
        List<ReportingAreaModel> itemsDataModelList = new();
        IList<ReportPositionGridForMasterProcessModel> gridsByProcessList =
            new List<ReportPositionGridForMasterProcessModel>();

        List<ReportingAreaModel> primaryAreaModelList;
        ReportPositionGridForMasterProcessModel gridsByProcess;

        if (mergeItemsPrimaryDataInOneArea)
        {
            (
                primaryAreaModelList
                , gridsByProcess
                ) = GetItemsPrimaryDataForMasterProcess(
                    AppConstants.FilterDefaultValueAllAvailableProcesses
                    , reportItemPrimaryInfoList
                    , reportItemLinkedPrimaryInfoList
                    , hasItemEndEditableField
                    );

            itemsDataModelList.AddRange(primaryAreaModelList);
            gridsByProcessList.Add(gridsByProcess);

            return (itemsDataModelList, gridsByProcessList);
        }
        //else
        //we have to create different data groups by distinct process available in results list
        foreach (long processId in reportItemPrimaryInfoList.Select(ipi => ipi.ProcessId)
                                                            .Distinct())
        {
            (
                primaryAreaModelList
                , gridsByProcess
                ) = GetItemsPrimaryDataForMasterProcess(
                    processId
                    , reportItemPrimaryInfoList.Where(ipi => ipi.ProcessId == processId)
                                               .ToList()
                    , reportItemLinkedPrimaryInfoList
                    , hasItemEndEditableField
                    );

            itemsDataModelList.AddRange(primaryAreaModelList);
            gridsByProcessList.Add(gridsByProcess);
        }
        return (itemsDataModelList, gridsByProcessList);
    }

    private (
        List<ReportingAreaModel> primaryAreaModelList
        , ReportPositionGridForMasterProcessModel gridsByProcess
        )
        GetItemsPrimaryDataForMasterProcess(
            long processId
            , IList<ReportItemPrimaryInfoModel> itemMasterPrimaryInfoList
            , IList<ReportItemPrimaryInfoModel> itemLinkedPrimaryInfoList
            , bool hasItemEndEditableField
            )
    {
        List<ReportingAreaModel> primaryAreaModelList = new();
        ReportPositionGridForMasterProcessModel gridByProcess = new()
        {
            MasterProcessId = processId,
            ItemsPositioningGrid = new List<ReportPositionRowItemModel>(),
        };

        ReportingAreaSimpleModel reportingAreaSimpleModel =
            _webReportingDataTable.GetPrimaryDataTableSchema(
                    ReportAreaDataToLoad.DataMainItemsByProcess
                    , itemMasterPrimaryInfoList
                    , hasItemEndEditableField
                    );

        ReportingAreaModel itemsPrimaryDataModel = new()
        {
            Type = ReportAreaDataToLoad.DataMainItemsByProcess,
            Data = reportingAreaSimpleModel.Data,
            StepIndex = 0,
            ProcessId = processId,
            ColumnFeatureSet = reportingAreaSimpleModel.ColumnFeatureSet,
        };
        DataRow tmpRow;
        int table4ProcRowIndex = 0;
        bool isFirstAddition = false;
        //all items and additional areas must be ordered by id
        foreach (ReportItemPrimaryInfoModel reportItemPrimaryInfo
            in itemMasterPrimaryInfoList.OrderBy(ibpr => ibpr.Id))
        {
            tmpRow =
                _webReportingDataTable.MapRowItemPrimaryData(
                    itemsPrimaryDataModel.Data
                    , reportItemPrimaryInfo
                    , hasItemEndEditableField
                    );

            itemsPrimaryDataModel.Data.Rows.Add(tmpRow);

            if (itemLinkedPrimaryInfoList
                    .Where(ilp => ilp.IdMaster == reportItemPrimaryInfo.Id)
                    .HasValues())
            {
                //for each main item there can be multiple linked items
                //so we need to add offset rows below each master item
                //so that linked items keep alignment to each other between report data areas
                isFirstAddition = false;
                foreach (ReportItemPrimaryInfoModel primaryInfoLinked
                            in itemLinkedPrimaryInfoList
                                    .Where(ilp => ilp.IdMaster == reportItemPrimaryInfo.Id)
                                    .OrderBy(ilp => ilp.Id))
                {
                    if (!isFirstAddition)
                    {
                        isFirstAddition = true;
                        gridByProcess.ItemsPositioningGrid.Add(
                           new ReportPositionRowItemModel
                           {
                               RowIndex = table4ProcRowIndex,
                               ItemIdMaster = primaryInfoLinked.IdMaster,
                               ProcessIdMaster = reportItemPrimaryInfo.ProcessId,
                               ItemIdLinked = primaryInfoLinked.Id,
                               ProcessIdLinked = primaryInfoLinked.ProcessId,
                           });
                    }
                    else //this is the case when there are more than one linked item to master item
                    {
                        table4ProcRowIndex++;//increase global table row index

                        gridByProcess.ItemsPositioningGrid.Add(
                           new ReportPositionRowItemModel
                           {
                               RowIndex = table4ProcRowIndex,
                               ItemIdMaster = primaryInfoLinked.IdMaster, //0, //set 0 if someone wants empty item row
                               ProcessIdMaster = reportItemPrimaryInfo.ProcessId,//0,//set 0 if someone wants empty item row
                               ItemIdLinked = primaryInfoLinked.Id,
                               ProcessIdLinked = primaryInfoLinked.ProcessId,
                           });
                        //tmpRow = itemsPrimaryDataModel.Data.NewRow();//add empty row//commented in case someone wants empty rows
                        tmpRow =
                            _webReportingDataTable.MapRowItemPrimaryData(
                                itemsPrimaryDataModel.Data
                                , reportItemPrimaryInfo
                                , hasItemEndEditableField
                                );
                        itemsPrimaryDataModel.Data.Rows.Add(tmpRow);//repeat row
                    }
                }
            }
            else//case when no items linked detected
            {
                gridByProcess.ItemsPositioningGrid.Add(
                    new ReportPositionRowItemModel
                    {
                        RowIndex = table4ProcRowIndex,
                        ItemIdMaster = reportItemPrimaryInfo.Id,
                        ProcessIdMaster = reportItemPrimaryInfo.ProcessId,
                        ItemIdLinked = 0,
                        ProcessIdLinked = 0,
                    });
            }

            table4ProcRowIndex++;
        }

        primaryAreaModelList.Add(itemsPrimaryDataModel);
        if (itemLinkedPrimaryInfoList.IsNullOrEmpty())
        {
            return (primaryAreaModelList, gridByProcess);
        }

        ReportingAreaSimpleModel reportingAreaSimpleLinkedModel =
            _webReportingDataTable.GetPrimaryDataTableSchema(
                    ReportAreaDataToLoad.DataMainLinkedItemsByProcess
                    , itemLinkedPrimaryInfoList
                    , hasItemEndEditableField
                    );

        ReportingAreaModel itemsLinkedPrimaryDataModel = new()
        {
            Type = ReportAreaDataToLoad.DataMainLinkedItemsByProcess,
            Data = reportingAreaSimpleLinkedModel.Data,
            StepIndex = 0,

            //we associate to master/linked process id instead of this linked item process
            ProcessId = processId,
            ColumnFeatureSet = reportingAreaSimpleLinkedModel.ColumnFeatureSet,
        };
        //using the grid object we build we can now fill the linked items area
        //the rows without item link will be added empty
        foreach (ReportPositionRowItemModel gridRow in
            gridByProcess.ItemsPositioningGrid.OrderBy(gr => gr.RowIndex))
        {
            if (gridRow.ItemIdLinked.Invalid())
            {
                tmpRow = itemsLinkedPrimaryDataModel.Data.NewRow();
            }
            else
            {
                IEnumerable<ReportItemPrimaryInfoModel> linkedItems =
                    itemLinkedPrimaryInfoList.Where(rilp => rilp.Id == gridRow.ItemIdLinked);
                if (linkedItems.IsNullOrEmpty() || linkedItems.Count() > 1)
                {
                    _logger.LogAppError("error in grid building");
                    throw new WebAppException();
                }
                tmpRow =
                    _webReportingDataTable.MapRowItemPrimaryData(
                        itemsLinkedPrimaryDataModel.Data
                        , linkedItems.First()
                        , hasItemEndEditableField
                        );
            }
            itemsLinkedPrimaryDataModel.Data.Rows.Add(tmpRow);
        }
        primaryAreaModelList.Add(itemsLinkedPrimaryDataModel);

        return (primaryAreaModelList, gridByProcess);
    }




    private IList<ReportingAreaModel> GetItemsStepsData(
        IList<ReportingAreaModel> dataToLoadAndReturn
        , IList<ReportPositionGridForMasterProcessModel> gridForProcessList
        , IList<ItemsFormsByProcessModel> itemFormDataByProcess
        , ReportAreaDataToLoad stepsAreaType
        )
    {
        if (itemFormDataByProcess.IsNullOrEmpty())
        {
            return dataToLoadAndReturn;
        }

        ReportingAreaModel tmpReportingSheetDataModel = null;
        DataRow tmpRow;
        ItemsFormsByProcessModel reportItemFormDataByProcess;
        IEnumerable<ItemFormDisplayBasicModel> tmpItemFormDataModelList;
        IOrderedEnumerable<long> orderedProgressivesList;
        long tmpFormStepProgressive;
        long tmpItemId;
        long tmpProcessId;
        long totalSteps;

        foreach (ReportPositionGridForMasterProcessModel processGrid in gridForProcessList)
        {
            //use max to get the correct process current grid
            switch (stepsAreaType)
            {
                case ReportAreaDataToLoad.DataStepItemsByProcess:
                    tmpProcessId = processGrid.ItemsPositioningGrid.Max(ipg => ipg.ProcessIdMaster);
                    break;

                case ReportAreaDataToLoad.DataStepLinkedItemsByProcess:
                    tmpProcessId = processGrid.ItemsPositioningGrid.Max(ipg => ipg.ProcessIdLinked);
                    break;

                default:
                    _logger.LogAppError($"wrong input type '{stepsAreaType}'");
                    throw new WebAppException();
            }

            reportItemFormDataByProcess =
                itemFormDataByProcess.FirstOrDefault(ifbp => ifbp.ProcessId == tmpProcessId);

            if (reportItemFormDataByProcess is null
                || reportItemFormDataByProcess.ItemsFormsByFormCodeList.IsNullOrEmpty())
            {
                continue;
            }

            totalSteps =
                    reportItemFormDataByProcess.ItemsFormsByFormCodeList
                                                    .Select(ifbp => ifbp.StepProgressive)
                                                    .Distinct()
                                                    .Count();
            if (totalSteps.Invalid())
            {
                continue;
            }


            //IMPORTANT: we start from progressive 1 because 0 is used for single type areas
            for (int stepProgressive = 1; stepProgressive <= totalSteps; stepProgressive++)
            {
                tmpReportingSheetDataModel =
                    GetStepAreaWhenDataFound(
                        tmpProcessId
                        , stepProgressive
                        , itemFormDataByProcess
                        , processGrid.MasterProcessId
                        , stepsAreaType
                        );

                if (tmpReportingSheetDataModel is null)
                {
                    continue;//step does not have data to be shown
                }


                foreach (ReportPositionRowItemModel gridRow in processGrid.ItemsPositioningGrid)
                {
                    switch (stepsAreaType)
                    {
                        case ReportAreaDataToLoad.DataStepItemsByProcess:
                            tmpItemId = gridRow.ItemIdMaster;
                            break;

                        case ReportAreaDataToLoad.DataStepLinkedItemsByProcess:
                            tmpItemId = gridRow.ItemIdLinked;
                            break;

                        default:
                            _logger.LogAppError($"configuration wrong: input type '{stepsAreaType}' ");
                            throw new WebAppException();
                    }

                    if (tmpItemId.Invalid())
                    {
                        tmpRow = tmpReportingSheetDataModel.Data.NewRow();
                        tmpReportingSheetDataModel.Data.Rows.Add(tmpRow);
                        continue;
                    }

                    foreach (ItemsFormsByProcessModel itemFormDataByProcessModel
                        in itemFormDataByProcess.Where(ifp => ifp.ProcessId == tmpProcessId))
                    {
                        if (itemFormDataByProcessModel is null
                            || itemFormDataByProcessModel.ItemsFormsByFormCodeList.IsNullOrEmpty())
                        {
                            continue;
                        }

                        //get the list of steps progressives for current process
                        orderedProgressivesList =
                            itemFormDataByProcessModel.ItemsFormsByFormCodeList
                                .Select(idfc => idfc.StepProgressive)
                                .Distinct()
                                .OrderBy(prg => prg);
                        if (orderedProgressivesList.Count() < stepProgressive)
                        {
                            continue;
                        }
                        //-1 because by construction stepProgressive starts at 1
                        tmpFormStepProgressive = orderedProgressivesList.ElementAt(stepProgressive - 1);


                        foreach (ItemsFormsByFormCodeModel itemFormDataByFormCodeModel
                                    in itemFormDataByProcessModel.ItemsFormsByFormCodeList
                                        .Where(idfc => idfc.StepProgressive == tmpFormStepProgressive))
                        {
                            if (itemFormDataByFormCodeModel is null
                                || itemFormDataByFormCodeModel.ItemFormDisplayBasicList.IsNullOrEmpty())
                            {
                                continue;
                            }


                            tmpItemFormDataModelList =
                                itemFormDataByFormCodeModel.ItemFormDisplayBasicList
                                                                .Where(ifd => ifd.IdItem == tmpItemId);

                            tmpRow =
                                _webReportingDataTable.MapRowFormStep(
                                    tmpReportingSheetDataModel.Data
                                    , tmpItemFormDataModelList
                                    );
                            ////empty row must be created and added anyway because a step for the item has still to start
                            //tmpRow = tmpReportingSheetDataModel.Data.NewRow();

                            //if (tmpItemFormDataModelList.HasValues())
                            //{
                            //    tmpItemDataForStep = tmpItemFormDataModelList.First();
                            //    tmpRow[_localizer[nameof(LocalizedStr.SharedFieldLabelFormDateLastEdit)]] =
                            //            tmpItemDataForStep.SubmitDate.ToStringShortDate(_contextApp.GetCurrentCulture());

                            //    tmpRow[_localizer[nameof(LocalizedStr.SharedFieldLabelFormUserLastEdit)]] =
                            //            tmpItemDataForStep.SubmitUserSurname
                            //            + CodeConstants.Space
                            //            + tmpItemDataForStep.SubmitUserName;


                            //    foreach (InputControlViewModel submittedInput in tmpItemDataForStep.SubmittedInputList)
                            //    {
                            //        //if (submittedInput.Attachments.HasValues())
                            //        //{
                            //        //    tmpRow[submittedInput.Description.GetStringContent()] =
                            //        //        string.Join(
                            //        //            CodeConstants.SemiColon + Environment.NewLine
                            //        //            , submittedInput.Attachments.Select(a => a.Name)
                            //        //            );
                            //        //}
                            //        //else
                            //        //{
                            //            tmpRow[submittedInput.Description.GetStringContent()] = submittedInput.DisplayValue;
                            //        //}
                            //    }
                            //}

                            tmpReportingSheetDataModel.Data.Rows.Add(tmpRow);
                        }
                    }
                }

                if (tmpReportingSheetDataModel != null)
                {
                    dataToLoadAndReturn.Add(tmpReportingSheetDataModel);
                }
            }
        }

        return dataToLoadAndReturn;
    }

    private ReportingAreaModel GetStepAreaWhenDataFound(
        long processId
        , int stepProgressive
        , IList<ItemsFormsByProcessModel> itemFormDataByProcess
        , long areaMasterProcessId
        , ReportAreaDataToLoad areaType
        )
    {
        ReportingAreaModel areaModel = null;
        IOrderedEnumerable<long> orederedProgressivesList;
        long tmpFormStepProgressive;
        ReportingAreaSimpleModel tmpReportItemStepFormSchemaModel;
        HashSet<ReportingColumnFeaturesModel> optionsForReport = new();

        foreach (ItemsFormsByProcessModel reportItemFormDataByProcessModel
            in itemFormDataByProcess.Where(ifp => ifp.ProcessId == processId))
        {
            if (reportItemFormDataByProcessModel is null
                    || reportItemFormDataByProcessModel.ItemsFormsByFormCodeList.IsNullOrEmpty())
            {
                continue;
            }

            //get the list of steps progressives for current process
            orederedProgressivesList =
                reportItemFormDataByProcessModel.ItemsFormsByFormCodeList.Select(idfc => idfc.StepProgressive)
                                                                         .Distinct()
                                                                         .OrderBy(prg => prg);
            if (orederedProgressivesList.Count() < stepProgressive)
            {
                continue;
            }
            //-1 because by construction stepProgressive starts at 1
            tmpFormStepProgressive = orederedProgressivesList.ElementAt(stepProgressive - 1);

            foreach (ItemsFormsByFormCodeModel itemFormDataByFormCodeModel
                    in reportItemFormDataByProcessModel.ItemsFormsByFormCodeList
                            .Where(idfc => idfc.StepProgressive == tmpFormStepProgressive))
            {
                if (itemFormDataByFormCodeModel is null
                        || itemFormDataByFormCodeModel.ItemFormDisplayBasicList.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (ItemFormDisplayBasicModel itemFormDataModel
                        in itemFormDataByFormCodeModel.ItemFormDisplayBasicList)
                {
                    if (itemFormDataModel != null
                        && itemFormDataModel.SubmittedInputList.HasValues())
                    {
                        tmpReportItemStepFormSchemaModel =
                            _webReportingDataTable.GetFormsDataTableSchema(
                                    itemFormDataByFormCodeModel
                                    );

                        areaModel = new ReportingAreaModel()
                        {
                            Data = tmpReportItemStepFormSchemaModel.Data,
                            ProcessId = areaMasterProcessId,
                            Type = areaType,
                            StepIndex = stepProgressive,
                            ColumnFeatureSet = tmpReportItemStepFormSchemaModel.ColumnFeatureSet,
                        };
                        return areaModel;
                    }
                }
            }
        }

        return areaModel;
    }
    #endregion



    public FileDownloadInfoModel BuildExcelDumpForStep(
        ItemFormViewModel itemFormSubmitViewModel
        , string formStepCode
        )
    {
        _logger.LogAppDebug("CALL");

        DateTime timeStamp = DateTime.Now;

        IList<ReportingAreaModel> dataToLoad = new List<ReportingAreaModel>
            {
                //get datatables for data areas
                new ReportingAreaModel
                {
                    Data =
                        _webReportingDataTable.BuildFileInfoTable(
                            _contextApp.GetCurrentCulture()
                            , timeStamp
                            ),
                    Type = ReportAreaDataToLoad.DocumentInfo,
                    StepIndex = 0,
                },
                new ReportingAreaModel
                {
                    Data =
                        _webReportingDataTable.BuildPrimaryDataTable(
                            _contextApp.GetCurrentCulture()
                            , itemFormSubmitViewModel
                            ),
                    Type = ReportAreaDataToLoad.DataMainSingleStepInfo,
                    StepIndex = 0,
                },
                new ReportingAreaModel
                {
                    Data =
                        _webReportingDataTable.BuildFieldsTable(
                            itemFormSubmitViewModel
                            ),
                    Type = ReportAreaDataToLoad.DataStepSingleInfoFields,
                    StepIndex = 0,
                },
            };

        ReportConfigFileLgc reportConfigFile =
            _logicReportConfig.GetForSingleItemSingleStep(
                formStepCode
                , defaultSheetLocalizedName:
                    _localizer[nameof(LocalizedStr.SharedExportSingleItemStepSheetDescription)]
                );

        FileDownloadInfoModel fileDownloadInfoModel =
           _webReportingMapper.MapFile(
               dataToLoad
               , reportConfigFile
               , timeStamp
               );

        return fileDownloadInfoModel;
    }

    /// <summary>
    /// create DataGrid from items information retrieved
    /// </summary>
    /// <param name="reportDataDumpModel"></param>
    /// <returns></returns>
    public ReportAdvancedModel BuildReportAdvanced(
        ReportExportDataModel reportDataDumpModel
        , bool hasItemEndEditableField
        )
    {
        _logger.LogAppDebug("CALL");

        DateTime timeStamp = DateTime.Now;

        IList<ReportingAreaModel> dataToLoad = new List<ReportingAreaModel>();

        //estrai dati master degli items e degli items collegati (sotto processo)
        (
            IList<ReportingAreaModel> primaryAreaModelList
            , IList<ReportPositionGridForMasterProcessModel> gridsByProcessList
            ) = GetPrimaryAreasAndGrids(
                reportDataDumpModel.ItemsBasicDataList
                , reportDataDumpModel.ReportLinkedItemPrimaryInfoList
                , mergeItemsPrimaryDataInOneArea: false //not possible for report advanced
                , hasItemEndEditableField
                );

        _logger.LogAppInformation("Serialized calculated report items positioning grids " + JsonConvert.SerializeObject(gridsByProcessList));

        dataToLoad =
            dataToLoad.Concat(primaryAreaModelList)
                      .ToList();

        //estrai dati del registro
        dataToLoad =
            GetItemsStepsData(
                dataToLoadAndReturn: dataToLoad
                , gridsByProcessList
                , reportDataDumpModel.ItemsFormsByProcessList
                , stepsAreaType: ReportAreaDataToLoad.DataStepItemsByProcess
                );

        //estrai dati del registro associato (sotto processo)
        dataToLoad =
            GetItemsStepsData(
                dataToLoadAndReturn: dataToLoad
                , gridsByProcessList
                , reportDataDumpModel.ItemLinkedFormDataByProcess
                , stepsAreaType: ReportAreaDataToLoad.DataStepLinkedItemsByProcess
                );

        //DANIELE
        DataTable dtMainTemp = new();
        //Se è sotto processo
        if (dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataStepLinkedItemsByProcess)
                      .Select(x => x.Data)
                      .Any())
        {
            dtMainTemp =
                dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataMainItemsByProcess)
                          .Select(x => x.Data)
                          .FirstOrDefault()
                          .Copy();

            var countMainLinkedItems = dtMainTemp.Columns.Count;
            foreach (DataColumn col in
                dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataMainLinkedItemsByProcess)
                          .Select(x => x.Data)
                          .FirstOrDefault()
                          .Columns)
            {
                if (dtMainTemp.Columns.Contains(col.ColumnName))
                {
                    dtMainTemp.Columns.Add(col.ColumnName + "_Registro_Correlato");
                }
                else
                {
                    dtMainTemp.Columns.Add(col.ColumnName);
                }
            }

            int countTempMainLinkedItems = countMainLinkedItems;
            for (int i = 0;
                        i < dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataMainItemsByProcess)
                                      .Select(x => x.Data)
                                      .FirstOrDefault().Rows.Count;
                        i++)
            {
                for (int ii = 0;
                    ii < dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataMainLinkedItemsByProcess)
                                   .Select(x => x.Data)
                                   .FirstOrDefault().Columns.Count;
                    ii++)
                {
                    dtMainTemp.Rows[i][countTempMainLinkedItems] =
                        dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataMainLinkedItemsByProcess)
                                  .Select(x => x.Data)
                                  .FirstOrDefault().Rows[i][ii];
                    countTempMainLinkedItems++;
                }
                countTempMainLinkedItems = countMainLinkedItems;
            }

            var countStepLinkedItems = dtMainTemp.Columns.Count;
            foreach (DataColumn col in
                dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataStepLinkedItemsByProcess)
                          .Select(x => x.Data)
                          .FirstOrDefault()
                          .Columns)
            {
                if (dtMainTemp.Columns.Contains(col.ColumnName))
                {
                    dtMainTemp.Columns.Add(col.ColumnName + "_Registro_Correlato");
                }
                else
                {
                    dtMainTemp.Columns.Add(col.ColumnName);
                }
            }

            int countTempStepLinkedItems = countStepLinkedItems;
            for (int i = 0;
                    i < dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataMainItemsByProcess)
                                  .Select(x => x.Data)
                                  .FirstOrDefault()
                                  .Rows.Count;
                    i++)
            {
                for (int ii = 0;
                    ii < dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataStepLinkedItemsByProcess)
                                   .Select(x => x.Data)
                                   .FirstOrDefault()
                                   .Columns.Count;
                    ii++)
                {
                    dtMainTemp.Rows[i][countTempStepLinkedItems] =
                        dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataStepLinkedItemsByProcess)
                                  .Select(x => x.Data)
                                  .FirstOrDefault()
                                  .Rows[i][ii];
                    countTempStepLinkedItems++;
                }
                countTempStepLinkedItems = countStepLinkedItems;
            }

        }
        else
        {
            //SE è processo
            dtMainTemp =
                dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataMainItemsByProcess)
                          .Select(x => x.Data)
                          .FirstOrDefault()
                          .Copy();
            var count = dtMainTemp.Columns.Count;
            //Creare un oggetto AreaModel con type DataItemsByProcess dove unisco il datatable DataMainItemsByProcess e DataStepItemsByProcess
            foreach (DataColumn col in
                dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataStepItemsByProcess)
                          .Select(x => x.Data)
                          .FirstOrDefault()
                          .Columns)
            {
                if (dtMainTemp.Columns.Contains(col.ColumnName))
                {
                    dtMainTemp.Columns.Add(col.ColumnName + "_1");
                }
                else
                {
                    dtMainTemp.Columns.Add(col.ColumnName);
                }
            }

            int countTemp = count;
            for (int i = 0;
                i < dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataMainItemsByProcess)
                              .Select(x => x.Data)
                              .FirstOrDefault()
                              .Rows.Count;
                            i++)
            {
                for (int ii = 0;
                    ii < dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataStepItemsByProcess)
                                   .Select(x => x.Data)
                                   .FirstOrDefault()
                                   .Columns.Count;
                    ii++)
                {
                    dtMainTemp.Rows[i][countTemp] =
                        dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataStepItemsByProcess)
                                  .Select(x => x.Data)
                                  .FirstOrDefault()
                                  .Rows[i][ii];
                    countTemp++;
                }
                countTemp = count;
            }
        }

        byte[] imageContent =
            File.ReadAllBytes(
                _webReportingMapper.GetImagePath(new ReportConfigAreaImageLgc() { ImageFromTenantContext = true })
                );
        ReportAdvancedModel output = new()
        {
            DataSourceGrid =
                JsonConvert.SerializeObject(
                    dataToLoad.Where(x => x.Type == ReportAreaDataToLoad.DataItemsByProcess)
                              .FirstOrDefault()
                              .Data
                    ),

            OptionsForReport = new(),

            TenantLogoToBase64 =
                Convert.ToBase64String(imageContent),
        };

        foreach (ReportingAreaModel dataArea in dataToLoad)
        {
            foreach (HashSet<ReportingColumnFeaturesModel> stepColumnOptions in
                        dataToLoad.Where(d => d.ColumnFeatureSet.HasValues())
                                  .Select(d => d.ColumnFeatureSet))
            {
                foreach (ReportingColumnFeaturesModel optionModel in stepColumnOptions)
                {
                    if (!output.OptionsForReport.Add(optionModel))
                    {
                        _logger.LogAppDebug($"found duplicated option for advanced report '{JsonConvert.SerializeObject(optionModel)}' ");
                    }
                }
            }
        }

        return output;
    }



    
    private void BuildAggregatedTable(
        ReportExportDataModel reportData
        , bool hasItemEndEditableField
        )
    {
        bool dontBuildLinkedColumns = reportData.ReportLinkedItemPrimaryInfoList.IsNullOrEmpty();

        /*obbiettivo: creare una tabella che contenga le 
         * 
         * colonne dati ITEM ROOT
         *      Esiste sempre
         *      
         * colonne dati passaggi SCHEDE ITEMS ROOT (costruire considerando solo i passaggi che hanno effettivamente dati).
         *      Può mancare se per nessuno dei passaggi è stato contrassegnato almeno un campo da visualizzare nel report
         * 
         * colonne dati ITEM LINKED TO ROOT
         *      Può mancare se non sono stati creati items collegati per nessuno degli items root
         *      Oppure non sono proprio previsti dalla configurazione
         *      
         * colonne dati passaggi SCHEDE ITEMS LINKED TO ROOT (costruire solo i passaggi che hanno effettivamente dati)
        */

        //get columns names for primary (same for each process, so they can be recycled)
        ReportingAreaSimpleModel itemsRootMainSchemaModel =
            _webReportingDataTable.GetPrimaryDataTableSchema(
                    isRootGroup: true
                    , reportData.ItemsBasicDataList
                    , hasItemEndEditableField
                    );
        if(itemsRootMainSchemaModel is null)
        {
            _logger.LogAppError($"root report items empty");
            throw new WebAppException();
        }

        //null if linked items list is empty
        ReportingAreaSimpleModel itemsLinkedMainSchemaModel =
            _webReportingDataTable.GetPrimaryDataTableSchema(
                    isRootGroup: false
                    , reportData.ReportLinkedItemPrimaryInfoList
                    , hasItemEndEditableField
                    );

        //null if forms list is empty
        Dictionary<long, List<StepFormSchema>> formsSchemasByProcess = 
            GetFormSchemasByProcess(reportData.ItemsFormsByProcessList);

        //now assemble one aggregated table for each process
        IOrderedEnumerable<ReportItemPrimaryInfoModel> tmpRootItemsMainData;
        DataTable tmpAggregatedTable;
        HashSet<ReportingColumnFeaturesModel> tmpAggregatedColumns;
        int columnsProgressive;
        bool hasLinked;
        IEnumerable<long> idsOfRootItemsForCurrentProcess;

        foreach (long processId in reportData.ItemsBasicDataList.Select(ri => ri.ProcessId)
                                                                      .Distinct()
                                                                      .OrderBy(pr => pr))
        {
            tmpRootItemsMainData = 
                reportData.ItemsBasicDataList.Where(ip => ip.ProcessId == processId)
                                                    .OrderBy(ip => ip.SubmitDateTime);
            if(tmpRootItemsMainData.IsNullOrEmpty())
            {
                continue;
            }

            tmpAggregatedTable = new DataTable()
            {
                TableName = "ItemsTable",
            };
            columnsProgressive = 0;
            tmpAggregatedColumns = new ();

            //add columns for root item main data
            tmpAggregatedTable.Columns.AddRange(itemsRootMainSchemaModel.DataColumns.ToArray());

            MergeColumnFeatures(
                itemsRootMainSchemaModel.ColumnFeatureSet
                , ref columnsProgressive
                , ref tmpAggregatedColumns
                );
            //foreach (ReportingColumnFeaturesModel colFeature in itemsRootMainSchemaModel.ColumnFeatureSet.OrderBy(cf=> cf.Progressive))
            //{
            //    tmpColFeature = colFeature.Copy();
            //    tmpColFeature.Progressive = columnsProgressive++;
            //    if(!tmpAggregatedColumns.Add(tmpColFeature))
            //    {
            //        _logger.LogAppError($"duplicated column {tmpColFeature.ColumnName}");
            //    };
            //}

            //add columns for root items form data (if they were found for current process)
            if (reportData.ItemsFormsByProcessList.HasValues() &&
                reportData.ItemsFormsByProcessList.Any(ifp => ifp.ProcessId == processId))
            {
                List<StepFormSchema> rootItemsStepForms = formsSchemasByProcess[processId];
                foreach (StepFormSchema stepForm in rootItemsStepForms.OrderBy(risf => risf.StepProgressive))
                {
                    if(stepForm.ColumsModel is null
                        || stepForm.ColumsModel.DataColumns.Count <= 0)
                    {
                        continue;
                    }
                    tmpAggregatedTable.Columns.AddRange(stepForm.ColumsModel.DataColumns.ToArray());

                    MergeColumnFeatures(
                        stepForm.ColumsModel.ColumnFeatureSet
                        , ref columnsProgressive
                        , ref tmpAggregatedColumns
                        );
                }
            }

            idsOfRootItemsForCurrentProcess = tmpRootItemsMainData.Select(ifp => ifp.Id);
            hasLinked = false;//reset

            //add columns for linked
            //(only if linked contain at least one item that has as master at least one root item filtered by current process)
            if (reportData.ReportLinkedItemPrimaryInfoList.HasValues()
                && reportData.ReportLinkedItemPrimaryInfoList.Select(il => il.IdMaster)
                                                             .Distinct()
                                                             .Intersect(idsOfRootItemsForCurrentProcess)
                                                             .HasValues())
            {
                hasLinked = true;
                tmpAggregatedTable.Columns.AddRange(itemsLinkedMainSchemaModel.DataColumns.ToArray());

                MergeColumnFeatures(
                    itemsLinkedMainSchemaModel.ColumnFeatureSet
                    , ref columnsProgressive
                    , ref tmpAggregatedColumns
                    );
            }

            //add columns for linked forms
            if(hasLinked                
                && reportData.ItemsFormsByProcessList.HasValues()
                && )
            {

            }

            //tmpAggregatedTable //output
            //tmpAggregatedColumns //output
        }
    }

    private void MergeColumnFeatures(
        HashSet<ReportingColumnFeaturesModel> columnsToMerge
        , ref int currentProgressive
        , ref HashSet<ReportingColumnFeaturesModel> currentAssembledColumns
        )
    {
        ReportingColumnFeaturesModel tmpColFeature;
        foreach (ReportingColumnFeaturesModel colFeature in columnsToMerge.OrderBy(cf => cf.Progressive))
        {
            tmpColFeature = colFeature.Copy();
            tmpColFeature.Progressive = currentProgressive++;
            if (!currentAssembledColumns.Add(tmpColFeature))
            {
                _logger.LogAppError($"duplicated column {tmpColFeature.ColumnName}");
            };
        }
    }

    private class StepFormSchema
    {
        internal string FormCode { get; set; }
        internal long StepProgressive { get; set; }
        internal IHtmlContent StepDescription { get; set; }
        internal IList<InputControlViewModel> SubmittedInputList { get; set; }
        internal ReportingAreaSimpleModel ColumsModel { get; set; }
    }

    //get columns names for linked (same for each process, so they can be recycled)
    private Dictionary<long, List<StepFormSchema>> GetFormSchemasByProcess(IList<ItemsFormsByProcessModel> itemFormDataByProcess)
    {
        if(itemFormDataByProcess.IsNullOrEmpty())
        {
            return null;
        }

        Dictionary<long, List<StepFormSchema>> formsByProcessOutput = new();
        StepFormSchema formSchema;
        List<StepFormSchema> formsSchema;
        string tmpFormCode;
        HashSet<string> formCodes = new();

        //get columns names for forms related to primary
        foreach (ItemsFormsByProcessModel formsByProcess in
                    itemFormDataByProcess.Where(fp =>
                        itemFormDataByProcess.Select(ip => ip.ProcessId)
                                             .Contains(fp.ProcessId))
                                         .OrderBy(fp => fp.ProcessId)
            )
        {
            formSchema = null;//reset

            if (formsByProcess.ItemsFormsByFormCodeList.IsNullOrEmpty())
            {
                continue;
            }

            formCodes = new();//reset
            formsSchema = new();//reset
            foreach (ItemsFormsByFormCodeModel formsByFormCode in
                            formsByProcess.ItemsFormsByFormCodeList.OrderBy(s => s.StepProgressive))
            {
                if (formsByFormCode.ItemFormDisplayBasicList.IsNullOrEmpty())
                {
                    continue;
                }

                tmpFormCode = formsByFormCode.FormCode;
                if (formCodes.Contains(tmpFormCode))
                {
                    continue;
                }
                formCodes.Add(tmpFormCode);

                formSchema = new()
                {
                    FormCode = tmpFormCode,
                    StepDescription = formsByFormCode.StepDescription,
                    StepProgressive = formsByFormCode.StepProgressive,
                };

                ReportingAreaSimpleModel reportingAreaSimple;
                foreach (ItemFormDisplayBasicModel form in formsByFormCode.ItemFormDisplayBasicList)
                {
                    reportingAreaSimple =
                        _webReportingDataTable.GetFormSchema(
                            formsByFormCode.StepDescription
                            , form
                            );
                    if (reportingAreaSimple is null)
                    {
                        continue;
                    }
                    formSchema.ColumsModel = reportingAreaSimple;
                    formSchema.SubmittedInputList = form.SubmittedInputList;
                    formsSchema.Add(formSchema);
                    break;
                }
            }

            formsByProcessOutput.Add(formsByProcess.ProcessId, formsSchema);
        }

        return formsByProcessOutput;
    }
}