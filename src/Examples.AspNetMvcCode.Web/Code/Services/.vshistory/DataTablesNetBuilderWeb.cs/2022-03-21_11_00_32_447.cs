namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// this class centralize the necessary code to convert data to json objects necessary
/// for javascript libraries
/// </summary>
public class DataTablesNetBuilderWeb : IDataTablesNetBuilderWeb
{
    private readonly ILogger<DataTablesNetBuilderWeb> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;

    private readonly ContextApp _contextApp;

    private readonly IUrlHelper _urlHelper;
    private readonly MainLocalizer _localizer;
    private readonly IPersonalizationWeb _webPersonalization;


    public DataTablesNetBuilderWeb(
        ILogger<DataTablesNetBuilderWeb> logger
        , IOptionsSnapshot<ProductSettings> optProduct
        , ContextApp contextApp
        , IUrlHelper urlHelper
        , MainLocalizer localizer
        , IPersonalizationWeb webPersonalization
        )
    {
        _logger = logger;
        _optProduct = optProduct;
        _contextApp = contextApp;
        _urlHelper = urlHelper;
        _localizer = localizer;
        _webPersonalization = webPersonalization;
    }

    private const string ColumnClassIndexerPrefix = "column-";
    private const int ColumnViewPriorityForMobileLowest = 999;


    public DataTablesNetViewModel BuildSupervisorSearchResultModel(
        IList<UserFoundModel> userFoundList
        )
    {
        _logger.LogAppDebug("CALL");

        DataTablesNetViewModel searchResult = new();
        if (userFoundList.IsNullOrEmpty())
        {
            return searchResult;
        }
        searchResult.AvailableColumnsIndexes = new Dictionary<string, int>();

        IList<SupervisorFoundJsonModel> supervisorFoundJsonList = new List<SupervisorFoundJsonModel>();
        IList<DataTablesNetColumnJsonModel> dataTableColumnsConfig = new List<DataTablesNetColumnJsonModel>();
        int index = 0;
        int columnIndex = 0;
        SupervisorFoundJsonModel tmpUserFound;
        foreach (UserFoundModel user in userFoundList)
        {
            tmpUserFound = new SupervisorFoundJsonModel();

            if (index == 0)
            {
                dataTableColumnsConfig.Add(
                    new DataTablesNetColumnJsonModel
                    {
                        Data = WebAppDataTablesNetConstants.LinkSupervisorModify,
                        Title = string.Empty,
                        ClassName = ColumnClassIndexerPrefix + columnIndex,
                        Orderable = false,
                        Searchable = false,
                        Visible = false,
                        ResponsivePriority = ColumnViewPriorityForMobileLowest,
                    }
                );
                searchResult.AvailableColumnsIndexes.Add(
                    WebAppDataTablesNetConstants.LinkSupervisorModify, columnIndex
                    );
                columnIndex++;
            }
            tmpUserFound.LinkSupervisorModify =
                _urlHelper.Action(
                    MvcComponents.ActSupervisorEdit
                    , MvcComponents.CtrlAccountAdministration
                    , new Dictionary<string, string>()
                        {
                                {ParamsNames.UserId , user.UserId.ToString()}
                        }
                    );


            if (index == 0)
            {
                dataTableColumnsConfig.Add(
                    new DataTablesNetColumnJsonModel
                    {
                        Data = WebAppDataTablesNetConstants.Login,
                        Title = _localizer[nameof(LocalizedStr.SharedFieldUserLogin)],
                        ClassName = ColumnClassIndexerPrefix + columnIndex,
                        Orderable = true,
                        Searchable = true,
                        Visible = true,
                        ResponsivePriority = 10,//not as relevant as other fields
                    }
                );
                searchResult.AvailableColumnsIndexes.Add(
                    WebAppDataTablesNetConstants.Login, columnIndex
                    );
                columnIndex++;
            }
            tmpUserFound.Login = user.Login;


            if (index == 0)
            {
                dataTableColumnsConfig.Add(
                    new DataTablesNetColumnJsonModel
                    {
                        Data = WebAppDataTablesNetConstants.Surname,
                        Title = _localizer[nameof(LocalizedStr.SharedFieldSurname)],
                        ClassName = ColumnClassIndexerPrefix + columnIndex,
                        Orderable = true,
                        Searchable = true,
                        Visible = true,
                        ResponsivePriority = 1,
                    }
                );
                searchResult.AvailableColumnsIndexes.Add(
                    WebAppDataTablesNetConstants.Surname, columnIndex
                    );
                columnIndex++;
            }
            tmpUserFound.Surname = user.Surname;


            if (index == 0)
            {
                dataTableColumnsConfig.Add(
                    new DataTablesNetColumnJsonModel
                    {
                        Data = WebAppDataTablesNetConstants.Name,
                        Title = _localizer[nameof(LocalizedStr.SharedFieldName)],
                        ClassName = ColumnClassIndexerPrefix + columnIndex,
                        Orderable = true,
                        Searchable = true,
                        Visible = true,
                        ResponsivePriority = 2,
                    }
                );
                searchResult.AvailableColumnsIndexes.Add(
                    WebAppDataTablesNetConstants.Name, columnIndex
                    );
                columnIndex++;
            }
            tmpUserFound.Name = user.Name;


            if (index == 0)
            {
                dataTableColumnsConfig.Add(
                    new DataTablesNetColumnJsonModel
                    {
                        Data = WebAppDataTablesNetConstants.Email,
                        Title = _localizer[nameof(LocalizedStr.SharedFieldEmail)],
                        ClassName = ColumnClassIndexerPrefix + columnIndex,
                        Orderable = true,
                        Searchable = true,
                        Visible = true,
                        ResponsivePriority = 3,
                    }
                );
                searchResult.AvailableColumnsIndexes.Add(
                    WebAppDataTablesNetConstants.Email, columnIndex
                    );
                columnIndex++;
            }
            tmpUserFound.Email = user.Email;


            if (index == 0)
            {
                dataTableColumnsConfig.Add(
                    new DataTablesNetColumnJsonModel
                    {
                        Data = WebAppDataTablesNetConstants.RoleType,
                        Title = string.Empty,
                        ClassName = ColumnClassIndexerPrefix + columnIndex,
                        Orderable = false,
                        Searchable = false,
                        Visible = false,
                        ResponsivePriority = ColumnViewPriorityForMobileLowest,
                    }
                );
                searchResult.AvailableColumnsIndexes.Add(
                    WebAppDataTablesNetConstants.RoleType, columnIndex
                    );
                columnIndex++;
            }
            tmpUserFound.RoleType = user.UserProfile.ToString();

            supervisorFoundJsonList.Add(tmpUserFound);
            index++;
        }
        searchResult.ColumnsJsonObjectSerialized =
            new HtmlString(JsonSerializer.Serialize(dataTableColumnsConfig, WebAppConstants.JsonObjectModelOptions));
        searchResult.RowsJsonObjectSerialized =
            new HtmlString(JsonSerializer.Serialize(supervisorFoundJsonList, WebAppConstants.JsonObjectModelOptions));

        return searchResult;
    }

    private const string DataTablesNetOrderAscending = "asc";
    //for now we don't have finely tuned personalization by tenant for search columns relative to item
    //only configuration by product
    //in case it's needed, code below must be adapted
    public DataTablesNetViewModel BuildItemSearchResultModel(
        IList<ItemAggregatedInfoModel> itemFoundList
        , bool hasItemEndEditableField
        )
    {
        _logger.LogAppDebug("CALL");

        DataTablesNetViewModel searchResult = new();
        if (itemFoundList.IsNullOrEmpty())
        {
            return searchResult;
        }
        searchResult.AvailableColumnsIndexes = new Dictionary<string, int>();


        (
            IList<DataTablesNetColumnJsonModel> dataTableColumnsConfig
            , IList<int> columnDefSimpleTargetsIndexes
            , IndexedDictionary<SearchResultItemField, int> columnsItemWithIndexes
            , IndexedDictionary<string, int> columnsFormWithIndexes
            , IList<PersonalizationGenericModel> formFieldPersonalization
            ) = BuildColumnsConfigurationForItemSearchResults(
                itemFoundList
                , hasItemEndEditableField
                );

        //here we have to build a data structure for rows
        //we can't use a static poco object because model can change
        //depending on which form field tenant want to see
        //only way to emulate a dynamic poco object is a list of dictionaries
        //where every entry is a key for property and value is the property value casted to string
        IList<Dictionary<string, string>> itemFoundTableDynamicList = new List<Dictionary<string, string>>();

        Dictionary<string, string> tableDataProperties;

        IOrderedEnumerable<int> orderedItemColumnsIndexes =
            columnsItemWithIndexes.Values.OrderBy(valueAsIndex => valueAsIndex);

        IOrderedEnumerable<int> orderedFormColumnsIndexes =
           columnsFormWithIndexes.Values.OrderBy(valueAsIndex => valueAsIndex);

        foreach (ItemAggregatedInfoModel item in itemFoundList)
        {
            tableDataProperties = new Dictionary<string, string>();

            //row part for item fields
            foreach (int columnIndex in orderedItemColumnsIndexes)
            {
                BuildRowItemPart(
                    ref tableDataProperties
                    , columnsItemWithIndexes
                    , item
                    , columnIndex
                    );
            }

            //row part for personalized form fields relative to item to show
            foreach (int columnIndex in orderedFormColumnsIndexes)
            {
                //TODO campo calcolati form
                BuildRowFormPart(
                    ref tableDataProperties
                    , columnsFormWithIndexes
                    , formFieldPersonalization
                    , item
                    , columnIndex
                    );
            }

            itemFoundTableDynamicList.Add(tableDataProperties);
        }




        searchResult.ColumnsJsonObjectSerialized =
            new HtmlString(JsonSerializer.Serialize(dataTableColumnsConfig, WebAppConstants.JsonObjectModelOptions));
        searchResult.RowsJsonObjectSerialized =
            new HtmlString(JsonSerializer.Serialize(itemFoundTableDynamicList, WebAppConstants.JsonObjectModelOptions));


        searchResult.AvailableColumnsIndexes = new Dictionary<string, int>();
        foreach (SearchResultItemField itemCol in columnsItemWithIndexes.Keys)
        {
            searchResult.AvailableColumnsIndexes.Add(
                WebAppUtility.ToSerializationCase(itemCol)
                , columnsItemWithIndexes[itemCol]
                );
        }
        foreach (string formCol in columnsFormWithIndexes.Keys)
        {
            searchResult.AvailableColumnsIndexes.Add(
                WebAppUtility.ToSerializationCase(formCol)
                , columnsFormWithIndexes[formCol]
                );
        }

        searchResult.ColumnDefSimpleTargetsSerialized =
            new HtmlString(JsonSerializer.Serialize(columnDefSimpleTargetsIndexes, WebAppConstants.JsonObjectModelOptions));

        List<List<string>> orderingModel = new();
        orderingModel.Add(
            new List<string>
            {
                columnsItemWithIndexes[SearchResultItemField.ProcessDescription].ToString(),
                DataTablesNetOrderAscending
            });
        orderingModel.Add(
            new List<string>
            {
                columnsItemWithIndexes[SearchResultItemField.DateTimeSubmit].ToString(),
                DataTablesNetOrderAscending
            });
        searchResult.ColumnDefaultOrderingModelSerialized =
            new HtmlString(JsonSerializer.Serialize(orderingModel, WebAppConstants.JsonObjectModelOptions));

        return searchResult;
    }




    #region columns setup

    private (
        IList<DataTablesNetColumnJsonModel> dataTableColumnsConfig
        , IList<int> columnDefSimpleTargetsIndexes
        , IndexedDictionary<SearchResultItemField, int> columnsItemWithIndexes
        , IndexedDictionary<string, int> columnsFormWithIndexes
        , IList<PersonalizationGenericModel> formFieldPersonalization
        ) BuildColumnsConfigurationForItemSearchResults(
            IList<ItemAggregatedInfoModel> itemFoundList
            , bool hasItemEndEditableField
            )
    {
        //extract the non-empty calculated field for each item to be sure to have all possible columns
        //using hash set to be sure that calculated columns will not be repeated
        HashSet<ItemCalculatedFieldViewModel> calculatedFieldsToShowAsColumns = new();

        IEnumerable<HashSet<ItemCalculatedFieldViewModel>> validCalculatedFieldsFound;
        foreach (ItemAggregatedInfoModel item in itemFoundList)
        {
            validCalculatedFieldsFound =
                    itemFoundList.Where(i => i.CalculatedFields.HasValues()
                                                && i.CalculatedFields.Any(c => c.Value.StringHasValue()))
                                 .Select(i => i.CalculatedFields);
            if (validCalculatedFieldsFound.IsNullOrEmpty())
            {
                continue;
            }
            foreach (HashSet<ItemCalculatedFieldViewModel> calculatedFields in validCalculatedFieldsFound)
            {
                foreach (ItemCalculatedFieldViewModel calculatedField in calculatedFields)
                {
                    calculatedFieldsToShowAsColumns.Add(calculatedField);
                }
            }

        }

        (
            IndexedDictionary<SearchResultItemField, ColumnDefinition> configuredColumns
            , IList<int> columnDefSimpleTargetsIndexes
            ) = GetItemColumnDefinitions(
                calculatedFieldsToShowAsColumns
                , hasItemEndEditableField
                );

        IList<DataTablesNetColumnJsonModel> dataTableColumnsConfig = new List<DataTablesNetColumnJsonModel>();
        foreach (SearchResultItemField columnKey in configuredColumns.Keys)
        {
            dataTableColumnsConfig.Add(
               new DataTablesNetColumnJsonModel()
               {
                   Data = WebAppUtility.ToSerializationCase(columnKey),
                   Title = configuredColumns[columnKey].Title,
                   ClassName = ColumnClassIndexerPrefix + configuredColumns[columnKey].Index,
                   Orderable = configuredColumns[columnKey].Orderable,
                   Searchable = configuredColumns[columnKey].Searchable,
                   Visible = configuredColumns[columnKey].Visible,
                   ResponsivePriority = configuredColumns[columnKey].Priority,
               }
               );
        }

        IndexedDictionary<SearchResultItemField, int> columnsItemWithIndexes = new();

        KeyValuePair<SearchResultItemField, ColumnDefinition> tmpColumn;
        foreach (ColumnDefinition col in configuredColumns.Values.OrderBy(ci => ci.Index))
        {
            tmpColumn = configuredColumns.First(ci => ci.Value.Index == col.Index);
            columnsItemWithIndexes.Add(tmpColumn.Key, tmpColumn.Value.Index);
        }


        //custom columns from form fields
        //IndexedDictionary<DynamicFormField, string> itemSearchResultPersonalizationList =

        (
            IList<PersonalizationGenericModel> formFieldPersonalization
            , IndexedDictionary<string, int> columnsFormWithIndexes
            ) = GetColumnsFormFields(
                ref dataTableColumnsConfig
                , ref columnDefSimpleTargetsIndexes
                , itemFoundList.First().FieldsLogicList
                , currentColumnIndex: columnsItemWithIndexes.Values.Max()
                );

        return (
            dataTableColumnsConfig
            , columnDefSimpleTargetsIndexes
            , columnsItemWithIndexes
            , columnsFormWithIndexes
            , formFieldPersonalization
            );
    }

    private class ColumnDefinition
    {
        internal int Index { get; set; }
        internal string Title { get; set; }
        internal bool Orderable { get; set; }
        public bool Searchable { get; set; }
        public bool Visible { get; set; }
        public int Priority { get; set; }
    }

    /// <summary>
    /// this columns should not be included in targets, because component build them in a different way
    /// </summary>
    private static readonly IList<SearchResultItemField> ColumnsToExcludeFromDefs =
        new List<SearchResultItemField> {
                SearchResultItemField.RequiresAttention,
                SearchResultItemField.LinkToModify,
                SearchResultItemField.ProcessDescription,
                SearchResultItemField.UnreadChatMessage,
                SearchResultItemField.ExpirationWarning,
        };

    private (
        IndexedDictionary<SearchResultItemField, ColumnDefinition> configuredColumns
        , IList<int> columnDefSimpleTargetsIndexes
        ) GetItemColumnDefinitions(
            HashSet<ItemCalculatedFieldViewModel> calculatedFieldsToShow
            , bool hasItemEndEditableField
            )
    {
        int columnIndex = 0;
        IndexedDictionary<SearchResultItemField, ColumnDefinition> availableColumnsIndexes = new()
        {
            //add fixed columns, non customizable
            {
                SearchResultItemField.RequiresAttention,
                new ColumnDefinition
                {
                    Index = columnIndex++,
                    Title = string.Empty,
                    Orderable = false,
                    Searchable = false,
                    Visible = false,
                    Priority = ColumnViewPriorityForMobileLowest,
                }
            },
            {
                SearchResultItemField.LinkToModify,
                new ColumnDefinition
                {
                    Index = columnIndex++,
                    Title = string.Empty,
                    Orderable = false,
                    Searchable = false,
                    Visible = false,
                    Priority = ColumnViewPriorityForMobileLowest,
                }
            },
            {
                SearchResultItemField.ProcessDescription,
                new ColumnDefinition
                {
                    Index = columnIndex++,
                    Title = string.Empty,
                    Orderable = true,
                    Searchable = true,
                    Visible = false,//will be shown in a row grouping
                    Priority = 1,
                }
            },
        };

        //unread chat message warning
        if (!_optProduct.Value.DisableUsersChatComponent)
        {
            availableColumnsIndexes.Add(
                SearchResultItemField.UnreadChatMessage
                , new ColumnDefinition
                {
                    Index = columnIndex++,
                    Title = string.Empty,
                    Orderable = true,
                    Searchable = false,//this is a symbol column, does not have sense to be searchable
                    Visible = true,
                    Priority = 1,
                }
                );
        }

        //column title for warning expiration symbol appears only if end date column will not be shown 
        //to preserve space
        availableColumnsIndexes.Add(
                SearchResultItemField.ExpirationWarning
                , new ColumnDefinition
                {
                    Index = columnIndex++,
                    Title =
                        _optProduct.Value.UseItemEndDateForExpiration
                            ? string.Empty
                            : _localizer[nameof(LocalizedStr.SearchTableColumnReportExpirationWarning)],
                    Orderable = true,
                    Searchable = false,//this is a symbol column, does not have sense to be searchable
                    Visible = true,
                    Priority = 5,
                }
                );

        if (calculatedFieldsToShow.HasValues())
        {
            foreach (var cf in calculatedFieldsToShow.OrderBy(cf => cf.Progressive))
            {
                availableColumnsIndexes.Add(
                    cf.FieldName.ToEnum<SearchResultItemField>()
                    , new ColumnDefinition
                    {
                        Index = columnIndex++,
                        Title = cf.Description.GetStringContent(),
                        Orderable = true,
                        Searchable = true,
                        Visible = true,
                        Priority = 7,
                    }
                );
            }
        }



        //standard column display order
        IList<int> columnDefSimpleTargetsIndexes = new List<int>();

        //attention: personalization completely overrides standard column definition
        //in both column presence and title
        IndexedDictionary<SearchResultItemField, string> itemSearchResultsColumnsItemFields =
            _webPersonalization.GetForSearchResultsColumnsItemFields();

        if (itemSearchResultsColumnsItemFields.HasValues())
        {
            foreach (SearchResultItemField key in itemSearchResultsColumnsItemFields.Keys)
            {
                availableColumnsIndexes.Add(
                    key
                    , new ColumnDefinition
                    {
                        Index = columnIndex,
                        Title = itemSearchResultsColumnsItemFields[key],
                        Orderable = true,
                        Searchable = true,
                        Visible = true,
                        Priority = 10,
                    }
                    );
                if (!ColumnsToExcludeFromDefs.Contains(key))
                {
                    columnDefSimpleTargetsIndexes.Add(columnIndex);
                }
                columnIndex++;
            }
        }
        else  //define standard configuration
        {
            GetDefaultCustomizableItemColumns(
                ref columnIndex
                , ref availableColumnsIndexes
                , ref columnDefSimpleTargetsIndexes
                , hasItemEndEditableField
                );
        }
        return (
            availableColumnsIndexes
            , columnDefSimpleTargetsIndexes
            );
    }


    private void GetDefaultCustomizableItemColumns(
        ref int columnIndex
        , ref IndexedDictionary<SearchResultItemField, ColumnDefinition> configuredColumns
        , ref IList<int> columnDefSimpleTargetsIndexes
        , bool hasItemEndEditableField
        )
    {
        //end date
        if (hasItemEndEditableField)//_optProduct.Value.UseItemEndDateForExpiration
        {
            configuredColumns.Add(
               SearchResultItemField.DateEnd
               , new ColumnDefinition
               {
                   Index = columnIndex,
                   Title = _localizer[nameof(LocalizedStr.SearchTableColumnReportExpirationWarning)],
                   Orderable = true,
                   Searchable = true,
                   Visible = true,
                   Priority = 2,
               }
               );
            if (!ColumnsToExcludeFromDefs.Contains(SearchResultItemField.DateEnd))
            {
                columnDefSimpleTargetsIndexes.Add(columnIndex);
            }
            columnIndex++;
        }


        //item code (process code per WB, codice protocollo per REG)      
        configuredColumns.Add(
               SearchResultItemField.Code
               , new ColumnDefinition
               {
                   Index = columnIndex,
                   Title = _localizer[nameof(LocalizedStr.SharedFieldLabelItemDescriptiveCode)],
                   Orderable = true,
                   Searchable = true,
                   Visible = true,
                   Priority = 1,
               }
               );
        if (!ColumnsToExcludeFromDefs.Contains(SearchResultItemField.Code))
        {
            columnDefSimpleTargetsIndexes.Add(columnIndex);
        }
        columnIndex++;


        //item submit date
        //in case _optProduct.Value.ShowItemSubmitDate
        configuredColumns.Add(
               SearchResultItemField.DateTimeSubmit
               , new ColumnDefinition
               {
                   Index = columnIndex,
                   Title = _localizer[nameof(LocalizedStr.SharedFieldLabelDateTimeSubmit)],
                   Orderable = true,
                   Searchable = true,
                   Visible = true,
                   Priority = 4,
               }
               );
        if (!ColumnsToExcludeFromDefs.Contains(SearchResultItemField.DateTimeSubmit))
        {
            columnDefSimpleTargetsIndexes.Add(columnIndex);
        }
        columnIndex++;



        configuredColumns.Add(
               SearchResultItemField.StepDescription
               , new ColumnDefinition
               {
                   Index = columnIndex,
                   Title = _localizer[nameof(LocalizedStr.SharedFieldLabelStep)],
                   Orderable = true,
                   Searchable = true,
                   Visible = true,
                   Priority = 3,
               }
               );
        if (!ColumnsToExcludeFromDefs.Contains(SearchResultItemField.StepDescription))
        {
            columnDefSimpleTargetsIndexes.Add(columnIndex);
        }
        columnIndex++;
    }


    private (
        IList<PersonalizationGenericModel> formFieldPersonalization
        , IndexedDictionary<string, int> columnsFormWithIndexes
        ) GetColumnsFormFields(
        ref IList<DataTablesNetColumnJsonModel> dataTableColumnsConfig
        , ref IList<int> columnDefSimpleTargetsIndexes
        , IList<FieldLogicModel> fieldsLogicList
        , int currentColumnIndex
        )
    {
        IList<PersonalizationGenericModel> formFieldPersonalization = new List<PersonalizationGenericModel>();
        IndexedDictionary<string, int> columnsFormWithIndexes = new();

        if (fieldsLogicList.IsNullOrEmpty())
        {
            return (
                formFieldPersonalization
                , columnsFormWithIndexes
                );
        }

        formFieldPersonalization = _webPersonalization.GetForSearchResultsColumnsFormFields();

        FieldLogicModel fieldLogic;
        string tmpColumnTitle;
        foreach (PersonalizationGenericModel fieldPersonalization in formFieldPersonalization.OrderBy(ffp => ffp.Progressive))
        {
            //we use string instead of enum, so we can enable for view every form field just 
            //doing inserts in personalization and form mapping for logic tables
            fieldLogic = fieldsLogicList.First(fl => fl.DynamicFormFieldKey == fieldPersonalization.PersonalizationKey);

            //if personalization contains a description, replace the field description in form
            //advice: always override it in personalization config
            //because form description is always too long to be used as table header
            tmpColumnTitle = fieldPersonalization.Content;
            tmpColumnTitle =
                tmpColumnTitle.Empty()
                ? fieldLogic.FieldDescription.GetStringContent()
                : tmpColumnTitle;

            dataTableColumnsConfig.Add(new DataTablesNetColumnJsonModel()
            {
                Data = WebAppUtility.ToSerializationCase(fieldPersonalization.PersonalizationKey),
                Title = tmpColumnTitle,
                ClassName = ColumnClassIndexerPrefix + currentColumnIndex,
                Orderable = true,
                Searchable = true,
                Visible = true,
                ResponsivePriority = 7,
            });
            columnsFormWithIndexes.Add(
                    fieldPersonalization.PersonalizationKey, currentColumnIndex
                    );

            //rule: enabled form fields for view are directly added
            columnDefSimpleTargetsIndexes.Add(currentColumnIndex);


            currentColumnIndex++;
        }
        return (
               formFieldPersonalization
               , columnsFormWithIndexes
               );
    }
    #endregion



    #region rows setup


    /// <summary>
    /// create the first part of the table row as a list of key values, where key is column name and value is the cell value
    /// </summary>
    /// <param name="columnsItemWithIndexes"></param>
    /// <param name="tableDataProperties">each of these ones are the property name and the value. 
    /// Necessary for dynamic serialization</param>
    /// <param name="item"></param>
    /// <param name="columnIndex"></param>
    private void BuildRowItemPart(
        ref Dictionary<string, string> tableDataProperties
        , IndexedDictionary<SearchResultItemField, int> columnsItemWithIndexes
        , ItemAggregatedInfoModel item
        , int columnIndex
        )
    {
        SearchResultItemField tmpItemColumnName = columnsItemWithIndexes.First(x => x.Value == columnIndex).Key;
        string tmpSerializationColumn = WebAppUtility.ToSerializationCase(tmpItemColumnName);
        switch (tmpItemColumnName)
        {
            case SearchResultItemField.RequiresAttention:
                //color for open / closed / to check
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.RequiresAttention.ToString().ToLowerInvariant()
                    );
                break;

            case SearchResultItemField.LinkToModify:
                //link to item management
                tableDataProperties.Add(
                    tmpSerializationColumn,
                    _urlHelper.Action(
                        MvcComponents.ActViewAndManage
                        , MvcComponents.CtrlItemManagement
                        , new Dictionary<string, string>()
                            {
                                    {ParamsNames.IdItem , item.Id.ToString()}
                            }
                        ).ToString()
                        );
                break;

            case SearchResultItemField.ProcessDescription:
                //hidden column(will be used for rows group): process description
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.ProcessDescription.GetStringContent()
                    );
                break;

            case SearchResultItemField.UnreadChatMessage:
                //unread message symbol
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.HasUnreadMessage.ToString().ToLowerInvariant()
                    );
                break;

            case SearchResultItemField.ExpirationWarning:
                //expired item symbol
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.Expired.ToString().ToLowerInvariant()
                    );
                break;

            case SearchResultItemField.DateEnd:
                //item expiration date
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.EndDate.ToStringShortDate(_contextApp.GetCurrentCulture())
                    );
                break;

            case SearchResultItemField.Code:
                //codice item (process code per WB, codice protocollo per REG)
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.ItemDescriptiveCode
                    );
                break;

            case SearchResultItemField.DateTimeSubmit:
                //data ora invio item
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.SubmitDateTime.ToStringDateTimeGeneralLong(_contextApp.GetCurrentCulture())
                    );
                break;

            case SearchResultItemField.StepDescription:
                //descrizione step corrente
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.CurrentStepDescription.GetStringContent()
                    );
                break;

            case SearchResultItemField.Calculation001:
            case SearchResultItemField.Calculation002:
            case SearchResultItemField.Calculation003:
            case SearchResultItemField.Calculation004:
            case SearchResultItemField.Calculation005:
            case SearchResultItemField.Calculation006:
            case SearchResultItemField.Calculation007:
            case SearchResultItemField.Calculation008:
            case SearchResultItemField.Calculation009:
            case SearchResultItemField.Calculation010:
                tableDataProperties.Add(
                    tmpSerializationColumn
                    , item.CalculatedFields.HasValues()
                        ? item.CalculatedFields.Where(cf => cf.FieldName.EqualsInvariant(tmpItemColumnName.ToString()))
                                               .Select(cf => cf.Value)
                                               .First()
                        : string.Empty
                    );
                break;

            default:
                _logger.LogAppError($"unhandled item column '{tmpItemColumnName}' found for item {item.Id} during row construction");
                throw new WebAppException();
        }
    }



    private void BuildRowFormPart(
        ref Dictionary<string, string> tableDataProperties
        , IndexedDictionary<string, int> columnsFormWithIndexes
        , IList<PersonalizationGenericModel> formFieldPersonalization
        , ItemAggregatedInfoModel item
        , int columnIndex
        )
    {
        if (formFieldPersonalization.IsNullOrEmpty())
        {
            _logger.LogAppError($"personalization definition empty for form fields to show in search, but columns are defined");
            throw new WebAppException();
        }

        string tmpFormColumnName = columnsFormWithIndexes.First(x => x.Value == columnIndex).Key;
        string tmpSerializationColumn = WebAppUtility.ToSerializationCase(tmpFormColumnName);

        //find item field by column name
        IEnumerable<FieldLogicModel> fieldByColumn =
            item.FieldsLogicList.Where(fl => fl.DynamicFormFieldKey == tmpFormColumnName);


        if (fieldByColumn.IsNullOrEmpty())
        {
            _logger.LogAppError($"no form fields for column '{tmpFormColumnName}' found in item {item.Id}");
            throw new WebAppException();
        }
        tableDataProperties.Add(
            tmpSerializationColumn
            , fieldByColumn.First().SavedValue
            );
    }
    #endregion
}