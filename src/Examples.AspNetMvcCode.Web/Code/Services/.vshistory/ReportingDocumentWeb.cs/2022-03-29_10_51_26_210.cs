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

        IList<ReportingAreaModel> reportingAreaAggregated = BuildAggregatedTable(reportDataDumpModel);
        dataToLoad = 
            dataToLoad.Concat(reportingAreaAggregated)
                      .ToList();

        ReportConfigFileLgc reportConfigFile =
            _logicReportConfig.GetForAllItems(
                defaultSheetLocalizedName: _localizer[nameof(LocalizedStr.ReportDumpItemsSheetName)]
                , reportingAreaAggregated.Select(ra => ra.ProcessId)
                                         .ToList()
                );

        FileDownloadInfoModel fileDownloadInfoModel =
            _webReportingMapper.MapFile(
                dataToLoad
                , reportConfigFile
                , timeStamp
                );

        return fileDownloadInfoModel;
    }


    public FileDownloadInfoModel BuildExcelDumpForStep(
        ItemFormViewModel itemFormViewModel
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
                            , itemFormViewModel
                            ),
                    Type = ReportAreaDataToLoad.DataMainSingleStepInfo,
                    StepIndex = 0,
                },
                new ReportingAreaModel
                {
                    Data =
                        _webReportingDataTable.BuildFieldsTable(
                            itemFormViewModel
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

        IList<ReportingAreaModel> reportingAreaAggregated = BuildAggregatedTable(reportDataDumpModel);

        byte[] imageContent =
            File.ReadAllBytes(
                _webReportingMapper.GetImagePath(new ReportConfigAreaImageLgc() { ImageFromTenantContext = true })
                );
        ReportAdvancedModel output = new()
        {
            DataSourceGrid =
                JsonConvert.SerializeObject(
                    reportingAreaAggregated.Where(x => x.Type == ReportAreaDataToLoad.DataItemsByProcess)
                                           .FirstOrDefault()
                                           .Data
                    ),

            OptionsForReport = new(),

            TenantLogoToBase64 =
                Convert.ToBase64String(imageContent),
        };

        foreach (ReportingAreaModel dataArea in reportingAreaAggregated)
        {
            foreach (HashSet<ReportingColumnFeaturesModel> stepColumnOptions in
                        reportingAreaAggregated.Where(d => d.ColumnFeatureSet.HasValues())
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



    private class ReportCompleteAggregate
    {
        internal DataTable Data { get; set; }
        internal HashSet<ReportingColumnFeaturesModel> ColumnFeaturesSet { get; set; }
    }
    private IList<ReportingAreaModel> BuildAggregatedTable(
        ReportExportDataModel reportData
        )
    {
        /*obiettivo: creare una tabella per ogni processo che contenga le 
         * 
         * colonne dati ITEM ROOT (attenzione che un item ROOT può essere un collegato (sotto processo)
         *      Esiste sempre
         *      
         * colonne dati passaggi SCHEDE ITEMS ROOT (costruire considerando solo i passaggi che hanno effettivamente dati).
         *      Può mancare se per nessuno dei passaggi è stato contrassegnato almeno un campo da visualizzare nel report
         * 
         * per tutti i processi slave (linked to ROOT)
         * colonne dati ITEM LINKED TO ROOT
         *      Può mancare se non sono stati creati items collegati per nessuno degli items root
         *      Oppure non sono proprio previsti dalla configurazione
         *      
         * colonne dati passaggi SCHEDE ITEMS LINKED TO ROOT (costruire solo i passaggi che hanno effettivamente dati)
        */

        //get columns names for primary (same for each process, so they can be recycled)
        HashSet<ReportingAreaSimpleModel> itemsBasicDataSchemasList =
            _webReportingDataTable.GetBasicDataSchemasByProcess(
                reportData.ItemsBasicDataList
                , reportData.UseDateExpiration
                );
        if(itemsBasicDataSchemasList is null)
        {
            _logger.LogAppError($"root report items empty");
            throw new WebAppException();
        }

        //_logger.LogAppDebug($"{JsonConvert.SerializeObject(itemsBasicDataSchemasList, Newtonsoft.Json.Formatting.Indented)}");

        IList<ReportingAreaModel> output = new List<ReportingAreaModel>();

        //null if forms list is empty
        Dictionary<long, List<StepFormSchema>> formsSchemasByProcess = 
            GetFormSchemasByProcess(
                reportData.ItemsFormsByProcessList
                );

        Dictionary<long, ReportCompleteAggregate> completeAggregateDict = new();
        ReportCompleteAggregate tmpCompleteAggregate;

        HashSet<ReportingColumnFeaturesModel> tmpAggregatedColumnsFeatureSet;
        int columnsProgressive;

        IOrderedEnumerable<ReportItemBasicDataModel> tmpRootItemsMainData;
        ReportingAreaSimpleModel tmpRootBasicSchema;
        List<StepFormSchema> tmpRootItemsStepForms;

        HashSet<long> tmpSlaveProcesses;
        ReportingAreaSimpleModel tmpSlaveBasicSchema;
        List<StepFormSchema> tmpSlaveItemsStepForms;

        int slaveItemRowCount;
        int tmpSlaveItemRowCountMax;
        DataRow tmpCompleteAggregateDataRow;
        Dictionary<long, List<long>> tmpSlaveIdBySlaveProcess;

        //now assemble one aggregated table for each root/starting process
        foreach (long rootProcessId in reportData.RootProcessIdList)
        {
            tmpRootItemsMainData = 
                reportData.ItemsBasicDataList.Where(ip => ip.ProcessId == rootProcessId)
                                             .OrderBy(ip => ip.SubmitDateTime);
            if(tmpRootItemsMainData.IsNullOrEmpty())
            {
                continue;
            }

            tmpCompleteAggregate = new()
            {
                Data = 
                    new DataTable()
                    {
                        TableName = "ItemsTable" + rootProcessId.ToString(),
                    },
            };

            columnsProgressive = 0;
            tmpAggregatedColumnsFeatureSet = new ();//we must use a var because it will be used as ref parameter

            tmpRootBasicSchema = itemsBasicDataSchemasList.First(i => i.ProcessId == rootProcessId);

            //add columns for root item main data
            tmpCompleteAggregate.Data.Columns.AddRange(tmpRootBasicSchema.DataColumns.Copy().ToArray());

            MergeColumnFeatures(
                tmpRootBasicSchema.ColumnFeatureSet
                , ref columnsProgressive
                , ref tmpAggregatedColumnsFeatureSet
                );

            //add columns for root items form data (if they were found for current process)
            if (reportData.ItemsFormsByProcessList.HasValues() &&
                reportData.ItemsFormsByProcessList.Any(ifp => ifp.ProcessId == rootProcessId))
            {
                tmpRootItemsStepForms = formsSchemasByProcess[rootProcessId];
                foreach (StepFormSchema rootStepForm in tmpRootItemsStepForms.OrderBy(risf => risf.StepProgressive))
                {
                    if(rootStepForm.ColumsModel is null
                        || rootStepForm.ColumsModel.DataColumns.Count <= 0)
                    {
                        continue;
                    }
                    tmpCompleteAggregate.Data.Columns.AddRange(rootStepForm.ColumsModel.DataColumns.Copy().ToArray());

                    MergeColumnFeatures(
                        rootStepForm.ColumsModel.ColumnFeatureSet
                        , ref columnsProgressive
                        , ref tmpAggregatedColumnsFeatureSet
                        );
                }
            }

            //build a complete list of all possible slave processes of current root process
            tmpSlaveProcesses = new();
            foreach (ReportItemBasicDataModel itemBasic in tmpRootItemsMainData)
            {
                if(itemBasic.ProcessIdSlaves.IsNullOrEmpty())
                {
                    continue;
                }
                foreach(long slaveProcess in itemBasic.ProcessIdSlaves)
                {
                    tmpSlaveProcesses.Add(slaveProcess);
                }
            }

            //for each slave process add slave basic item columns, followed by relative forms, ordering by  slave process
            foreach(long slaveProcessId in tmpSlaveProcesses.OrderBy(sp => sp))
            {
                if(!itemsBasicDataSchemasList.Any(i => i.ProcessId == slaveProcessId))
                {
                    continue;
                }
                tmpSlaveBasicSchema = itemsBasicDataSchemasList.First(i => i.ProcessId == slaveProcessId);
                tmpCompleteAggregate.Data.Columns.AddRange(tmpSlaveBasicSchema.DataColumns.Copy().ToArray());

                MergeColumnFeatures(
                    tmpSlaveBasicSchema.ColumnFeatureSet
                    , ref columnsProgressive
                    , ref tmpAggregatedColumnsFeatureSet
                    );

                if (reportData.ItemsFormsByProcessList.HasValues() &&
                    reportData.ItemsFormsByProcessList.Any(ifp => ifp.ProcessId == slaveProcessId))
                {
                    tmpSlaveItemsStepForms = formsSchemasByProcess[slaveProcessId];
                    foreach (StepFormSchema stepForm in tmpSlaveItemsStepForms.OrderBy(risf => risf.StepProgressive))
                    {
                        if (stepForm.ColumsModel is null
                            || stepForm.ColumsModel.DataColumns.Count <= 0)
                        {
                            continue;
                        }
                        tmpCompleteAggregate.Data.Columns.AddRange(stepForm.ColumsModel.DataColumns.Copy().ToArray());

                        MergeColumnFeatures(
                            stepForm.ColumsModel.ColumnFeatureSet
                            , ref columnsProgressive
                            , ref tmpAggregatedColumnsFeatureSet
                            );
                    }
                }
            }

            //unique row progressive, necessary for client js component
            tmpCompleteAggregate.Data.Columns.Add(
                new DataColumn()
                {
                    DataType = typeof(long),
                    ColumnName = "Index",
                });
            tmpAggregatedColumnsFeatureSet.Add(
                new ReportingColumnFeaturesModel()
                {
                    ColumnName = "Index",
                    Progressive = columnsProgressive++,
                    Visible = false,
                });

            tmpCompleteAggregate.ColumnFeaturesSet = tmpAggregatedColumnsFeatureSet;
            //schema build for each table

            completeAggregateDict.Add(rootProcessId, tmpCompleteAggregate);

        }//end cycle root processes to build aggregated datatable columns


        IOrderedEnumerable<ReportItemBasicDataModel> tmpItemsBasicDataByRootProcess;

        //add rows to each aggregated datatable
        foreach (long rootProcessId in reportData.RootProcessIdList)
        {
            if(!completeAggregateDict.ContainsKey(rootProcessId))
            {
                continue;
            }

            tmpItemsBasicDataByRootProcess = 
                reportData.ItemsBasicDataList.Where(ib => ib.ProcessId == rootProcessId)
                                             ?.OrderByDescending(ib => ib.SubmitDateTime);

            if(tmpItemsBasicDataByRootProcess.IsNullOrEmpty())
            {
                string message = $"basic items datatable model has been build but no items found for root process '{rootProcessId}'. Should not have happened";
                _logger.LogAppError(message);
                throw new WebException(message);
            }

            //get model for this root process
            tmpCompleteAggregate = completeAggregateDict[rootProcessId];

            int indexRow = 1;
            foreach (ReportItemBasicDataModel itemBasicData in tmpItemsBasicDataByRootProcess)
            {
                //find all items slaves by slave process and create as many row as the max items between slave process
                //repeat main item data as many rows are reacted
                //don't repeat the slave items data because each one is related to master/root, not to each other slave

                tmpSlaveIdBySlaveProcess = new();
                slaveItemRowCount = 0;
                if(itemBasicData.ProcessIdSlaves.IsNullOrEmpty())
                {
                    slaveItemRowCount = 1;
                }
                else
                {
                    foreach (long slaveProcessId in itemBasicData.ProcessIdSlaves)
                    {
                        IEnumerable<ReportItemBasicDataModel> itemSlavesForProcessSlaveFound =
                            reportData.ItemsBasicDataList.Where(ps => ps.ProcessId == slaveProcessId
                                                                    && itemBasicData.IdSlaves.Contains(ps.Id));
                        tmpSlaveItemRowCountMax = itemSlavesForProcessSlaveFound.Count();
                        if (tmpSlaveItemRowCountMax > slaveItemRowCount)
                        {
                            slaveItemRowCount = tmpSlaveItemRowCountMax;
                        }

                        tmpSlaveIdBySlaveProcess.Add(
                            slaveProcessId
                            , itemSlavesForProcessSlaveFound.OrderByDescending(isps => isps.SubmitDateTime)
                                                            .Select(isps => isps.Id)
                                                            .ToList()
                            );
                    }
                }
                

                //now we have the number of rows to create 
                //single root item rows. It must be repeated, slave items not
                //also each slave process items must be beside each other
                for (int index = 0; index < slaveItemRowCount; index++)
                {
                    tmpCompleteAggregateDataRow = tmpCompleteAggregate.Data.NewRow();
                    tmpCompleteAggregateDataRow["Index"] = indexRow++;

                    //map basic data of root item
                    tmpCompleteAggregateDataRow = 
                        _webReportingDataTable.MapRowItemBasicData(
                            tmpCompleteAggregateDataRow
                            , itemBasicData
                            , omitSuffix: false //true
                            , reportData.UseDateExpiration
                            );

                    //map all form step of root item
                    IEnumerable<ItemsFormsByProcessModel> formsItemsByProcessFound = 
                        reportData.ItemsFormsByProcessList.Where(ifp => ifp.ProcessId == itemBasicData.ProcessId);
                    if(formsItemsByProcessFound.HasValues())
                    {
                        ItemsFormsByProcessModel formsItems = formsItemsByProcessFound.First();
                        foreach(ItemsFormsByFormCodeModel stepForms in formsItems.ItemsFormsByFormCodeList)//no need for ordering
                        {
                            IEnumerable<ItemFormDisplayBasicModel> formItemFound = stepForms.ItemFormDisplayBasicList.Where(sp => sp.IdItem == itemBasicData.Id);
                            if (formItemFound.IsNullOrEmpty())
                            {
                                continue;
                            }

                            ItemFormDisplayBasicModel formItem = formItemFound.First();
                            //root items forms do not use process description in columns names 
                            tmpCompleteAggregateDataRow =
                                _webReportingDataTable.MapRowForm(
                                    tmpCompleteAggregateDataRow
                                    , formsItems.ProcessDescription//, new HtmlString(string.Empty)
                                    , stepForms.StepDescription
                                    , formItemFound.First()
                                    );
                        }
                    }

                    //map one slave item for each slave process when exists
                    foreach(long slaveProcess in tmpSlaveIdBySlaveProcess.Keys)
                    {
                        if(tmpSlaveIdBySlaveProcess[slaveProcess].IsNullOrEmpty())
                        {
                            continue;
                        }

                        IOrderedEnumerable<ReportItemBasicDataModel> itemsSlaveFound = 
                            reportData.ItemsBasicDataList.Where(ib => ib.ProcessId == slaveProcess
                                                                && tmpSlaveIdBySlaveProcess[slaveProcess].Contains(ib.Id))
                                                         ?.OrderByDescending(ib => ib.SubmitDateTime);
                        if(itemsSlaveFound.IsNullOrEmpty())
                        {
                            continue;
                        }

                        ReportItemBasicDataModel itemSlave = itemsSlaveFound.First();

                        tmpCompleteAggregateDataRow =
                            _webReportingDataTable.MapRowItemBasicData(
                                tmpCompleteAggregateDataRow
                                , itemSlave
                                , omitSuffix: false
                                , reportData.UseDateExpiration
                                );

                        IEnumerable<ItemsFormsByProcessModel> formsItemsBySlaveProcessFound =
                           reportData.ItemsFormsByProcessList.Where(ifp => ifp.ProcessId == itemSlave.ProcessId);
                        if (formsItemsBySlaveProcessFound.HasValues())
                        {
                            ItemsFormsByProcessModel formsItems = formsItemsBySlaveProcessFound.First();
                            foreach (ItemsFormsByFormCodeModel stepForms in formsItems.ItemsFormsByFormCodeList)//no need for ordering
                            {
                                IEnumerable<ItemFormDisplayBasicModel> formItemFound = stepForms.ItemFormDisplayBasicList.Where(sp => sp.IdItem == itemSlave.Id);
                                if (formItemFound.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                tmpCompleteAggregateDataRow =
                                    _webReportingDataTable.MapRowForm(
                                        tmpCompleteAggregateDataRow
                                        , formsItems.ProcessDescription
                                        , stepForms.StepDescription
                                        , formItemFound.First()
                                        );
                            }
                        }

                        //once item slave is mapped, remove it from tracking object
                        tmpSlaveIdBySlaveProcess[slaveProcess].Remove(itemSlave.Id);//check if it gets removed in dictionary , else it will not work
                    }


                    tmpCompleteAggregate.Data.Rows.Add(tmpCompleteAggregateDataRow);
                }
            }
            //for current root process

            output.Add(
                new ReportingAreaModel()
                {
                    Type = ReportAreaDataToLoad.DataItemsByProcess,
                    ProcessId = rootProcessId,
                    ColumnFeatureSet = tmpCompleteAggregate.ColumnFeaturesSet,
                    Data = tmpCompleteAggregate.Data,
                });
        }
        return output;
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

    //da esportare su parte datatable web quando finito
    //get columns names for linked (same for each process, so they can be recycled)
    private Dictionary<long, List<StepFormSchema>> GetFormSchemasByProcess(
        IList<ItemsFormsByProcessModel> itemFormDataByProcess
        )
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
        IHtmlContent processDescription;

        //get columns names for forms related to primary
        foreach (ItemsFormsByProcessModel formsByProcess in
                    itemFormDataByProcess
                        .Where(fp =>
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

            //add process description to columns names only for linked items
            processDescription =
                //rootProcessIdList.Contains(formsByProcess.ProcessId)
                //? new HtmlString(string.Empty)
                //: 
                formsByProcess.ProcessDescription;

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
                            processDescription
                            , formsByFormCode.StepDescription
                            , form
                            , formsByFormCode.SaveOptionsDescriptionInsteadOfValue
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