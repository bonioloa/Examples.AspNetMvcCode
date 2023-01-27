using Newtonsoft.Json;

namespace Comunica.ProcessManager.Web.Code;

public class ReportingDataTableWeb : IReportingDataTableWeb
{
    private readonly ILogger<ReportingDataTableWeb> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;
    private readonly ContextApp _contextApp;
    private readonly ContextUser _contextUser;


    private readonly IUserConfiguratorLogic _logicUserConfigurator;

    private readonly MainLocalizer _localizer;

    public ReportingDataTableWeb(
        ILogger<ReportingDataTableWeb> logger
        , IOptionsSnapshot<ProductSettings> optProduct
        , ContextApp contextApp
        , ContextUser contextUser
        , IUserConfiguratorLogic logicUserConfigurator
        , MainLocalizer localizer
        )
    {
        _logger = logger;
        _optProduct = optProduct;
        _contextApp = contextApp;
        _contextUser = contextUser;
        _logicUserConfigurator = logicUserConfigurator;
        _localizer = localizer;
    }


    public DataTable BuildFileInfoTable(CultureInfo culture, DateTime creationTimestamp)
    {
        _logger.LogAppDebug("CALL");

        DataColumn tmpColumn;
        DataRow tmpRow;

        DataTable tmpTable = new()
        {
            TableName = _localizer[nameof(LocalizedStr.SharedFieldLabelDocumentInformation)],
        };
        tmpColumn = new DataColumn
        {
            DataType = typeof(string),
            ColumnName = MainTbColPlhDescription,
        };
        tmpTable.Columns.Add(tmpColumn);

        tmpColumn = new DataColumn
        {
            DataType = typeof(string),
            ColumnName = MainTbColPlhInformation,
        };
        tmpTable.Columns.Add(tmpColumn);

        (string userSurname, string userName)
            = _logicUserConfigurator.GetCurrentUserData();

        tmpRow = tmpTable.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelDateTimeSubmit)];
        tmpRow[MainTbColPlhInformation] = creationTimestamp.ToStringDateTimeGeneralLong(culture);
        tmpTable.Rows.Add(tmpRow);

        tmpRow = tmpTable.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelUserSurname)];
        tmpRow[MainTbColPlhInformation] = userSurname;
        tmpTable.Rows.Add(tmpRow);

        tmpRow = tmpTable.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelUserName)];
        tmpRow[MainTbColPlhInformation] = userName;
        tmpTable.Rows.Add(tmpRow);

        tmpRow = tmpTable.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelRoles)];
        tmpRow[MainTbColPlhInformation] =
            string.Join(
                CodeConstants.CommaStr + CodeConstants.SpaceStr
                , _contextUser.SupervisorRolesList.Select(sr => sr.Description.GetStringContent())
                );
        tmpTable.Rows.Add(tmpRow);

        return tmpTable;
    }



    public DataTable GetDataTableFiltersUsed(
        ReportExportDataModel reportDataDumpModel
        , bool getForAllAvailableProcesses
        , string dateSubmitFromStr
        , string dateSubmitToStr
        , bool hasItemEndEditableField
        , string dateExpirationFromStr
        , string dateExpirationToStr
        )
    {
        _logger.LogAppDebug("CALL");

        DataTable dataFilters = GetDataTableSchemaFilters(hasItemEndEditableField);
        DataRow tmpRow;

        tmpRow = dataFilters.NewRow();
        tmpRow[_localizer[nameof(LocalizedStr.SharedFieldLabelProcess)]] =
            GetDescriptionFilterProcess(
                reportDataDumpModel
                , getForAllAvailableProcesses
                );

        bool tmpDateConversionSuccess;

        DateTime? dateSubmitFrom;
        (tmpDateConversionSuccess, dateSubmitFrom) = dateSubmitFromStr.TryParseSortableDateInvariantToNullable();
        if (tmpDateConversionSuccess && dateSubmitFrom != null)
        {
            tmpRow[_localizer[nameof(LocalizedStr.SharedFieldLabelDateSubmitFrom)]] =
                dateSubmitFrom.Value.ToStringShortDate(_contextApp.GetCurrentCulture());
        }

        DateTime? dateSubmitTo;
        (tmpDateConversionSuccess, dateSubmitTo) = dateSubmitToStr.TryParseSortableDateInvariantToNullable();
        if (tmpDateConversionSuccess && dateSubmitTo != null)
        {
            tmpRow[_localizer[nameof(LocalizedStr.SharedFieldLabelDateSubmitTo)]] =
                dateSubmitTo.Value.ToStringShortDate(_contextApp.GetCurrentCulture());
        }

        if (hasItemEndEditableField)
        {
            DateTime? dateExpirationFrom;
            (tmpDateConversionSuccess, dateExpirationFrom) = dateExpirationFromStr.TryParseSortableDateInvariantToNullable();
            if (tmpDateConversionSuccess && dateExpirationFrom != null)
            {
                tmpRow[_localizer[nameof(LocalizedStr.SharedFieldLabelDateExpirationFrom)]] =
                    dateExpirationFrom.Value.ToStringShortDate(_contextApp.GetCurrentCulture());
            }

            DateTime? dateExpirationTo;
            (tmpDateConversionSuccess, dateExpirationTo) = dateExpirationToStr.TryParseSortableDateInvariantToNullable();
            if (tmpDateConversionSuccess && dateExpirationTo != null)
            {
                tmpRow[_localizer[nameof(LocalizedStr.SharedFieldLabelDateExpirationTo)]] =
                    dateExpirationTo.Value.ToStringShortDate(_contextApp.GetCurrentCulture());
            }
        }
        dataFilters.Rows.Add(tmpRow);
        return dataFilters;
    }

    private DataTable GetDataTableSchemaFilters(bool hasItemEndEditableField)
    {
        DataTable tbOutput = new()
        {
            TableName = _localizer[nameof(LocalizedStr.ReportDumpFiltersUsed)],
        };


        tbOutput.Columns.Add(new DataColumn
        {
            DataType = typeof(string),
            ColumnName = _localizer[nameof(LocalizedStr.SharedFieldLabelProcess)],
        });
        tbOutput.Columns.Add(new DataColumn
        {
            DataType = typeof(string),
            ColumnName = _localizer[nameof(LocalizedStr.SharedFieldLabelDateSubmitFrom)],
        });
        tbOutput.Columns.Add(new DataColumn
        {
            DataType = typeof(string),
            ColumnName = _localizer[nameof(LocalizedStr.SharedFieldLabelDateSubmitTo)],
        });

        if (hasItemEndEditableField)
        {
            tbOutput.Columns.Add(new DataColumn
            {
                DataType = typeof(string),
                ColumnName = _localizer[nameof(LocalizedStr.SharedFieldLabelDateExpirationFrom)],
            });
            tbOutput.Columns.Add(new DataColumn
            {
                DataType = typeof(string),
                ColumnName = _localizer[nameof(LocalizedStr.SharedFieldLabelDateExpirationTo)],
            });
        }

        return tbOutput;
    }

    private string GetDescriptionFilterProcess(
        ReportExportDataModel reportDataDumpModel
        , bool getForAllAvailableProcesses
        )
    {
        string processFiltersDescription = "";
        if (getForAllAvailableProcesses)
        {
            processFiltersDescription =
                _localizer[nameof(LocalizedStr.ReportAllProcesses)]
                + CodeConstants.ColonStr + Environment.NewLine;//+ AppConstants.ColonStr + AppConstants.SpaceStr;
        }
        processFiltersDescription +=
            string.Join(
                Environment.NewLine//AppConstants.CommaStr + AppConstants.SpaceStr
                , reportDataDumpModel.ItemsBasicDataList
                                     .Select(ripi => ripi.ProcessDescription.GetStringContent())
                                     .Distinct()
                );
        return processFiltersDescription;
    }


    private static string FormatColumnNameWithSuffix(string columnName, string suffix)
    {
        if(suffix.Empty())
        {
            return columnName;
        }
        return $"{columnName} ({suffix})";
    }
    private static string FormatColumnNameWithSuffixes(string columnName, string suffixPrimary, string suffixSecondary)
    {
        if (suffixPrimary.Empty() || suffixSecondary.Empty())
        {
            return columnName;
        }
        return $"{columnName} ({suffixSecondary}) ({suffixPrimary})";
    }
    private static string FormatColumnNameWithSuffix(IHtmlContent columnName, string suffix)
    {
        return FormatColumnNameWithSuffix(columnName.GetStringContent(), suffix);
    }
    private static string FormatColumnNameWithSuffixes(IHtmlContent columnName, string suffixPrimary, string suffixSecondary)
    {
        return FormatColumnNameWithSuffixes(columnName.GetStringContent(), suffixPrimary, suffixSecondary);
    }

    private void TestForColumnNameConflict(ref HashSet<string> alreadyUsedColumns, string columnToAdd)
    {
        if(!alreadyUsedColumns.Add(columnToAdd))
        {
            string message = $"column name '{columnToAdd}' already assigned. Current assigned columns '{System.Text.Json.JsonSerializer.Serialize(alreadyUsedColumns)}' ";
            _logger.LogAppError(message);
            throw new WebException(message);
        }
    }

    public HashSet<ReportingAreaSimpleModel> GetBasicDataSchemasByProcess(//corretto
        IList<ReportItemBasicDataModel> reportItemBasicDataList
        , bool hasItemEndEditableField
        )
    {
        _logger.LogAppDebug("CALL");

        if (reportItemBasicDataList.IsNullOrEmpty())
        {
            return null;
        }

        HashSet<ReportingAreaSimpleModel> output = new();

        foreach(ReportItemBasicDataModel reportItemBasic in reportItemBasicDataList.DistinctBy(i => i.ProcessId))
        {
            //convention: add process description to columns
            //if current process has items with master (is linked)
            //we ignore the fact that linked process can be used as root item for report
            IHtmlContent processDescription = reportItemBasic.ProcessDescription;
                //reportItemBasicDataList.Where(i => i.ProcessId == reportItemBasic.ProcessId)
                //                       .Any(i => i.IdMaster.Valid())
                //        ? reportItemBasic.ProcessDescription
                //        : new HtmlString(string.Empty);

            output.Add(
                 GetBasicDataSchema(
                    reportItemBasic.ProcessId
                    , processDescription
                    , hasItemEndEditableField
                    , reportItemBasicDataList.First().CalculatedFields
                    )
                );
        }

        return output;
    }

    private ReportingAreaSimpleModel GetBasicDataSchema(
        long processId
        , IHtmlContent processDescription
        , bool hasItemEndEditableField
        , HashSet< ItemCalculatedFieldViewModel> calculatedFields
        )
    {
        HashSet<string> alreadyUsedColumnsForGroup = new();

        string columnsSuffixPrimary = processDescription.GetStringContent();

        ReportingAreaSimpleModel output = new()
        {
            ProcessId = processId,
            ColumnsSuffixPrimary = columnsSuffixPrimary,
            DataColumns = new(),
            ColumnFeatureSet = new(),
        };

        string tmpColumnName = string.Empty;
        int progressive = 0;

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelItemInsertDateTime)], columnsSuffixPrimary);
        TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
        output.DataColumns.Add(
            new DataColumn
            {
                DataType = typeof(string),
                ColumnName = tmpColumnName,
            });
        output.ColumnFeatureSet.Add(
            new ReportingColumnFeaturesModel
            {
                ColumnName = tmpColumnName,
                Progressive = progressive++,
                Visible = true,
            });

        //this column must not be shown for whistleblowing report
        if (_optProduct.Value.ShowInReportItemInsertUser)
        {
            tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelItemInsertUser)], columnsSuffixPrimary);
            TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
            output.DataColumns.Add(
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = tmpColumnName,
                });
            output.ColumnFeatureSet.Add(
                new ReportingColumnFeaturesModel
                {
                    ColumnName = tmpColumnName,
                    Progressive = progressive++,
                    Visible = true,
                });
        }

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelProcess)], columnsSuffixPrimary);
        TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
        output.DataColumns.Add(
            new DataColumn
            {
                DataType = typeof(string),
                ColumnName = tmpColumnName,
            });
        output.ColumnFeatureSet.Add(
            new ReportingColumnFeaturesModel
            {
                ColumnName = tmpColumnName,
                Progressive = progressive++,
                Visible = true,
            });

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelItemId)], columnsSuffixPrimary);
        TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
        output.DataColumns.Add(
            new DataColumn
            {
                DataType = typeof(string),
                ColumnName = tmpColumnName,
            });
        output.ColumnFeatureSet.Add(
            new ReportingColumnFeaturesModel
            {
                ColumnName = tmpColumnName,
                Progressive = progressive++,
                Visible = true,
            });

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelItemDescriptiveCode)], columnsSuffixPrimary);
        TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
        output.DataColumns.Add(
            new DataColumn
            {
                DataType = typeof(string),
                ColumnName = tmpColumnName,
            });
        output.ColumnFeatureSet.Add(
            new ReportingColumnFeaturesModel
            {
                ColumnName = tmpColumnName,
                Progressive = progressive++,
                Visible = true,
            });

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelStep)], columnsSuffixPrimary);
        TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
        output.DataColumns.Add(
            new DataColumn
            {
                DataType = typeof(string),
                ColumnName = tmpColumnName,
            });
        output.ColumnFeatureSet.Add(
            new ReportingColumnFeaturesModel
            {
                ColumnName = tmpColumnName,
                Progressive = progressive++,
                Visible = true,
            });

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelStepStateGroup)], columnsSuffixPrimary);
        TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
        output.DataColumns.Add(
            new DataColumn
            {
                DataType = typeof(string),
                ColumnName = tmpColumnName,
            });
        output.ColumnFeatureSet.Add(
            new ReportingColumnFeaturesModel
            {
                ColumnName = tmpColumnName,
                Progressive = progressive++,
                Visible = true,
            });

        if (hasItemEndEditableField)
        {
            tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelDateExpiration)], columnsSuffixPrimary);
            TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
            output.DataColumns.Add(
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = tmpColumnName,
                });
            output.ColumnFeatureSet.Add(
                new ReportingColumnFeaturesModel
                {
                    ColumnName = tmpColumnName,
                    Progressive = progressive++,
                    Visible = true,
                });
        }

        foreach (var calculatedField in calculatedFields.OrderBy(cf => cf.Progressive))
        {
            tmpColumnName = FormatColumnNameWithSuffix(calculatedField.Description, columnsSuffixPrimary);
            TestForColumnNameConflict(ref alreadyUsedColumnsForGroup, tmpColumnName);
            output.DataColumns.Add(
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = tmpColumnName,
                });
            output.ColumnFeatureSet.Add(
                new ReportingColumnFeaturesModel
                {
                    ColumnName = tmpColumnName,
                    Progressive = progressive++,
                    Visible = true,
                });
        }
        return output;
    }

    
    public DataRow MapRowItemBasicData(
        DataRow row
        , ReportItemBasicDataModel reportItemInfo
        , bool omitSuffix
        , bool hasItemEndEditableField
        )
    {
        _logger.LogAppDebug("CALL");

        string columnsSuffixPrimary = 
            omitSuffix 
            ? string.Empty 
            : reportItemInfo.ProcessDescription.GetStringContent();

        string tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelItemInsertDateTime)], columnsSuffixPrimary);
        row[tmpColumnName] =
            reportItemInfo.SubmitDateTime.ToStringDateTimeGeneralLong(_contextApp.GetCurrentCulture());

        //this column must not be shown for whistleblowing report
        if (_optProduct.Value.ShowInReportItemInsertUser)
        {
            tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelItemInsertUser)], columnsSuffixPrimary);
            row[tmpColumnName] =
                reportItemInfo.SubmitUserSurname
                + CodeConstants.Space
                + reportItemInfo.SubmitUserName;
        }

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelProcess)], columnsSuffixPrimary);
        row[tmpColumnName] =
            reportItemInfo.ProcessDescription.ToString();

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelItemId)], columnsSuffixPrimary);
        row[tmpColumnName] =
            reportItemInfo.Id;

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelItemDescriptiveCode)], columnsSuffixPrimary);
        row[tmpColumnName] =
            reportItemInfo.ItemDescriptiveCode;

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelStep)], columnsSuffixPrimary);
        row[tmpColumnName] =
            reportItemInfo.CurrentStepDescription.ToString();

        tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelStepStateGroup)], columnsSuffixPrimary);
        row[tmpColumnName] =
            (reportItemInfo.StepStateGroup) switch
            {
                StepStateGroupType.Open => _localizer[nameof(LocalizedStr.SharedStepStateGroupOpen)],
                StepStateGroupType.Aborted => _localizer[nameof(LocalizedStr.SharedStepStateGroupAborted)],
                StepStateGroupType.Closed => _localizer[nameof(LocalizedStr.SharedStepStateGroupClosed)],
                _ => string.Empty,
            };
        if (hasItemEndEditableField)
        {
            tmpColumnName = FormatColumnNameWithSuffix(_localizer[nameof(LocalizedStr.SharedFieldLabelDateExpiration)], columnsSuffixPrimary);
            row[tmpColumnName] =
                reportItemInfo.ExpirationDate.ToStringShortDate(_contextApp.GetCurrentCulture());
        }

        foreach (var calculatedField in reportItemInfo.CalculatedFields) //no need for ordering here
        {
            tmpColumnName = FormatColumnNameWithSuffix(calculatedField.Description, columnsSuffixPrimary);
            row[tmpColumnName] =
                calculatedField.Value;
        }

        return row;
    }


    public ReportingAreaSimpleModel GetFormSchema(
        IHtmlContent processDescription
        , IHtmlContent stepDescription
        , ItemFormDisplayBasicModel itemForm
        , bool saveOptionsDescriptionInsteadOfValue
        )
    {
        _logger.LogAppDebug("CALL");

        if(itemForm is null || itemForm.SubmittedInputList.IsNullOrEmpty())
        {
            return null;
        }

        string suffixPrimary = processDescription.GetStringContent();
        string suffixSecondary = stepDescription.GetStringContent();
        ReportingAreaSimpleModel output = new()
        {
            ColumnFeatureSet = new(),
            ColumnsSuffixPrimary = suffixPrimary,
            ColumnsSuffixSecondary = suffixSecondary,
            DataColumns = new (),
        };

        HashSet<string> alreadyUsedColumns = new();

        string tmpColumnName = string.Empty;
        int progressive = 0;

        tmpColumnName = 
            FormatColumnNameWithSuffixes(
                _localizer[nameof(LocalizedStr.SharedFieldLabelFormDateLastEdit)]
                , suffixPrimary
                , suffixSecondary
                );
        TestForColumnNameConflict(ref alreadyUsedColumns, tmpColumnName);
        output.DataColumns.Add(
            new DataColumn
            {
                DataType = typeof(string),
                ColumnName = tmpColumnName,
            });
        output.ColumnFeatureSet.Add(
            new ReportingColumnFeaturesModel
            {
                ColumnName = tmpColumnName,
                Progressive = progressive++,
                Visible = true,
            });

        tmpColumnName = 
            FormatColumnNameWithSuffixes(
                _localizer[nameof(LocalizedStr.SharedFieldLabelFormUserLastEdit)]
                , suffixPrimary
                , suffixSecondary
                );
        TestForColumnNameConflict(ref alreadyUsedColumns, tmpColumnName);
        output.DataColumns.Add(
            new DataColumn
            {
                DataType = typeof(string),
                ColumnName = tmpColumnName,
            });
        output.ColumnFeatureSet.Add(
            new ReportingColumnFeaturesModel
            {
                ColumnName = tmpColumnName,
                Progressive = progressive++,
                Visible = true,
            });

        //PreventDtColumnNamesConflict(ref stepFormDataModel);

        ReportingColumnFeaturesModel tmpColumnFeature;
        foreach (InputControlViewModel input in 
                    itemForm.SubmittedInputList.OrderBy(f => f.Progressive))
        {

            tmpColumnName = FormatColumnNameWithSuffixes(
                input.Description
                , suffixPrimary
                , suffixSecondary
                );
            TestForColumnNameConflict(ref alreadyUsedColumns, tmpColumnName);

            output.DataColumns.Add(
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = tmpColumnName,
                });
            tmpColumnFeature =
                 new ReportingColumnFeaturesModel
                 {
                     ColumnName = tmpColumnName,
                     Progressive = progressive++,
                     Visible = true,
                 };

            //for now generate options only for the ones with valid colors
            if (input.ControlType.IsOption()
                && input.ChoiceOptions.Any(o => o.ColorValue.StringHasValue()))
            {
                tmpColumnFeature.SerializedOptionValues =
                    JsonConvert.SerializeObject(
                        //saveOptionsDescriptionInsteadOfValue
                        //? 
                        input.ChoiceOptions.Select(o => o.Description.GetStringContent())
                        //: input.ChoiceOptions.Select(o => o.Value)
                        );
                tmpColumnFeature.SerializedOptionColors =
                    JsonConvert.SerializeObject(input.ChoiceOptions.Select(o => o.ColorValue));
            }

            output.ColumnFeatureSet.Add(tmpColumnFeature);
        }


        return output;
    }

    //corretto
    public DataRow MapRowForm(
        DataRow row
        , IHtmlContent processDescription
        , IHtmlContent stepDescription
        , ItemFormDisplayBasicModel itemForm
        )
    {
        _logger.LogAppDebug("CALL");

        if (itemForm is null)
        {
            return row;
        }

        string suffixPrimary = processDescription.GetStringContent();
        string suffixSecondary = stepDescription.GetStringContent();

        string tmpColumnName =
            FormatColumnNameWithSuffixes(
                _localizer[nameof(LocalizedStr.SharedFieldLabelFormDateLastEdit)]
                , suffixPrimary
                , suffixSecondary
                );
        row[tmpColumnName] =
                itemForm.SubmitDate.ToStringShortDate(_contextApp.GetCurrentCulture());

        tmpColumnName =
            FormatColumnNameWithSuffixes(
                _localizer[nameof(LocalizedStr.SharedFieldLabelFormUserLastEdit)]
                 , suffixPrimary
                , suffixSecondary
                );
        row[tmpColumnName] =
                itemForm.SubmitUserSurname
                + CodeConstants.Space
                + itemForm.SubmitUserName;


        foreach (InputControlViewModel submittedInput in itemForm.SubmittedInputList)
        {
            tmpColumnName =
                FormatColumnNameWithSuffixes(
                    submittedInput.Description
                    , suffixPrimary
                    , suffixSecondary
                    );
            row[tmpColumnName] = submittedInput.DisplayValue;
        }

        return row;
    }

    //public DataRow MapRowFormStep(
    //    DataTable formModelTable
    //    , IEnumerable<ItemFormDisplayBasicModel> itemFormDataModelList
    //    )
    //{
    //    _logger.LogAppDebug("CALL");

    //    DataRow tmpRow = formModelTable.NewRow();

    //    ItemFormDisplayBasicModel tmpItemDataForStep;
    //    if (itemFormDataModelList.IsNullOrEmpty())
    //    {
    //        //row is added empty to maintain table structure, even in case this step is not yet completed
    //        return tmpRow;
    //    }

    //    tmpItemDataForStep = itemFormDataModelList.First();

    //    string tmpColumnName =
    //        FormatColumnNameWithSuffix(
    //            _localizer[nameof(LocalizedStr.SharedFieldLabelFormDateLastEdit)]
    //            , formModelTable.TableName
    //            );
    //    tmpRow[tmpColumnName] =
    //            tmpItemDataForStep.SubmitDate.ToStringShortDate(_contextApp.GetCurrentCulture());

    //    tmpColumnName =
    //        FormatColumnNameWithSuffix(
    //            _localizer[nameof(LocalizedStr.SharedFieldLabelFormUserLastEdit)]
    //            , formModelTable.TableName
    //            );
    //    tmpRow[tmpColumnName] =
    //            tmpItemDataForStep.SubmitUserSurname
    //            + CodeConstants.Space
    //            + tmpItemDataForStep.SubmitUserName;


    //    foreach (InputControlViewModel submittedInput in tmpItemDataForStep.SubmittedInputList)
    //    {
    //        tmpColumnName = FormatColumnNameWithSuffix(submittedInput.Description, formModelTable.TableName);
    //        tmpRow[tmpColumnName] = submittedInput.DisplayValue;
    //    }

    //    return tmpRow;
    //}

    //columns names can be duplicated in configuration so we add spaces to make them logically different
    //private static void PreventDtColumnNamesConflict(
    //    ref ItemsFormsByFormCodeModel stepFormDataModel
    //    )
    //{
    //    HashSet<string> columnsNamesFormInput = new();
    //    bool addSuccess;
    //    string fieldDescription;
    //    foreach (InputControlViewModel formInput in
    //        stepFormDataModel.ItemFormDisplayBasicList[0].SubmittedInputList)
    //    {
    //        addSuccess = false; //reset
    //        while (!addSuccess)
    //        {
    //            fieldDescription = formInput.Description.GetStringContent();
    //            addSuccess = columnsNamesFormInput.Add(fieldDescription);
    //            if (!addSuccess)
    //            {
    //                formInput.Description = new HtmlString(fieldDescription + CodeConstants.Space);
    //            }
    //        }
    //    }
    //}



    private const string MainTbColPlhDescription = "DESCRIPTION";
    private const string MainTbColPlhInformation = "INFORMATION";
    private const string DataTbColPlhField = "FIELD";
    private const string DataTbColPlhValue = "VALUE";
    public DataTable BuildPrimaryDataTable(
        CultureInfo culture
        , ItemFormViewModel itemFormSubmitViewModel
        )
    {
        DataColumn tmpColumn;
        DataRow tmpRow;

        DataTable tbMainData = new()
        {
            TableName = _localizer[nameof(LocalizedStr.SharedFieldLabelStepInformations)],
        };

        tmpColumn = new DataColumn
        {
            DataType = typeof(string),
            ColumnName = MainTbColPlhDescription,
            Caption = _localizer[nameof(LocalizedStr.SharedFieldLabelDescription)].ToString().ToUpper(),
        };
        tbMainData.Columns.Add(tmpColumn);

        tmpColumn = new DataColumn
        {
            DataType = typeof(string),
            ColumnName = MainTbColPlhInformation,
            Caption = _localizer[nameof(LocalizedStr.SharedFieldLabelInformation)].ToString().ToUpper()
        };
        tbMainData.Columns.Add(tmpColumn);

        tmpRow = tbMainData.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelItemDescriptiveCode)];
        tmpRow[MainTbColPlhInformation] = itemFormSubmitViewModel.ItemDescriptiveCode;
        tbMainData.Rows.Add(tmpRow);

        tmpRow = tbMainData.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelProcess)];
        tmpRow[MainTbColPlhInformation] = itemFormSubmitViewModel.ProcessDescription.GetStringContent();
        tbMainData.Rows.Add(tmpRow);

        tmpRow = tbMainData.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelStep)];
        tmpRow[MainTbColPlhInformation] = itemFormSubmitViewModel.StepDescription.GetStringContent();
        tbMainData.Rows.Add(tmpRow);

        tmpRow = tbMainData.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelItemInsertDateTime)];
        tmpRow[MainTbColPlhInformation] =
            itemFormSubmitViewModel.SubmitDate.ToStringDateTimeGeneralLong(culture);
        tbMainData.Rows.Add(tmpRow);

        //this column must not be shown for whistleblowing report
        if (_optProduct.Value.ShowInReportItemInsertUser)
        {
            tmpRow = tbMainData.NewRow();
            tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelItemInsertUser)];
            tmpRow[MainTbColPlhInformation] =
                itemFormSubmitViewModel.SubmitUserSurname
                    + CodeConstants.Space
                    + itemFormSubmitViewModel.SubmitUserName;
            tbMainData.Rows.Add(tmpRow);
        }

        tmpRow = tbMainData.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelUserSurnameInsert)];
        tmpRow[MainTbColPlhInformation] = itemFormSubmitViewModel.SubmitUserSurname;
        tbMainData.Rows.Add(tmpRow);

        tmpRow = tbMainData.NewRow();
        tmpRow[MainTbColPlhDescription] = _localizer[nameof(LocalizedStr.SharedFieldLabelUserNameInsert)];
        tmpRow[MainTbColPlhInformation] = itemFormSubmitViewModel.SubmitUserName;
        tbMainData.Rows.Add(tmpRow);

        //localize columns names
        tbMainData.Columns[MainTbColPlhDescription].ColumnName =
            _localizer[nameof(LocalizedStr.SharedFieldLabelDescription)].ToString().ToUpper();
        tbMainData.Columns[MainTbColPlhInformation].ColumnName =
            _localizer[nameof(LocalizedStr.SharedFieldLabelInformation)].ToString().ToUpper();

        return tbMainData;
    }


    public DataTable BuildFieldsTable(ItemFormViewModel itemFormSubmitViewModel)
    {
        _logger.LogAppDebug("CALL");

        DataColumn tmpColumn;
        DataRow tmpRow;
        DataTable tbFieldsData = new();

        tmpColumn = new DataColumn
        {
            DataType = typeof(string),
            ColumnName = DataTbColPlhField,
        };
        tbFieldsData.Columns.Add(tmpColumn);

        tmpColumn = new DataColumn
        {
            DataType = typeof(string),
            ColumnName = DataTbColPlhValue,
        };
        tbFieldsData.Columns.Add(tmpColumn);

        foreach (InputControlViewModel ctrl in itemFormSubmitViewModel.SubmittedInputList.OrderBy(f => f.Progressive))
        {
            tmpRow = tbFieldsData.NewRow();
            tmpRow[DataTbColPlhField] = ctrl.Description.GetStringContent();
            tmpRow[DataTbColPlhValue] = ctrl.DisplayValue;
            tbFieldsData.Rows.Add(tmpRow);
        }
        tbFieldsData.Columns[DataTbColPlhField].ColumnName =
            _localizer[nameof(LocalizedStr.SharedColumnFieldDescription)].ToString().ToUpper();
        tbFieldsData.Columns[DataTbColPlhValue].ColumnName =
            _localizer[nameof(LocalizedStr.SharedColumnFieldValue)].ToString().ToUpper();

        return tbFieldsData;
    }
}