namespace Examples.AspNetMvcCode.Web.Code;

public class ChartJsBuilderWeb : IChartJsBuilderWeb
{
    private readonly ILogger<ChartJsBuilderWeb> _logger;

    private readonly IUrlHelper _urlHelper;

    public ChartJsBuilderWeb(
        ILogger<ChartJsBuilderWeb> logger
        , IUrlHelper urlHelper
        )
    {
        _logger = logger;
        _urlHelper = urlHelper;
    }



    public ChartJsJsonModel BuildReportStatisticsModel(
        IList<ItemStepStatisticModel> itemStepStatisticList
        , long processId
        , DateTime? dateSubmitFrom
        , DateTime? dateSubmitTo
        , DateTime? dateExpirationFrom
        , DateTime? dateExpirationTo
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(BuildReportStatisticsModel) }
                });

        _logger.LogDebug("CALL");



        ChartJsJsonModel chartJsJsonModel = new();

        if (itemStepStatisticList.IsNullOrEmpty())
        {
            return chartJsJsonModel;
        }


        IList<string> sliceValueList = new List<string>();
        IList<string> stepDescriptionList = new List<string>();
        IList<ChartSliceLabelToStepSearchLinkJsonModel> graphSliceToItemSearchList =
            new List<ChartSliceLabelToStepSearchLinkJsonModel>();

        int index = 0;

        foreach (ItemStepStatisticModel stat in itemStepStatisticList)
        {
            sliceValueList.Add(stat.Count.ToString());
            stepDescriptionList.Add(stat.StepDescription.GetStringContent()); //we are forced to covert back to string

            //for each step description we include a direct link to search
            //that show the items that match selected slice criteria
            graphSliceToItemSearchList.Add(
                new ChartSliceLabelToStepSearchLinkJsonModel()
                {
                    StepDescription = stat.StepDescription.GetStringContent(), //we are forced to covert back to string

                    LinkToStepItemsSearch =
                        _urlHelper.Action(
                            MvcComponents.ActSearchNew
                            , MvcComponents.CtrlSearch
                            , new Dictionary<string, string>()
                                {
                                    { ParamsNames.ProcessId, processId.ToString()},
                                    { ParamsNames.StepStateGroup, StepStateGroupType.All.ToString() },
                                    { ParamsNames.ProcessStep, stat.StepCode},
                                    { ParamsNames.DateSubmitFrom, dateSubmitFrom.ToStringDateSortableInvariant()},
                                    { ParamsNames.DateSubmitTo, dateSubmitTo.ToStringDateSortableInvariant()},
                                    { ParamsNames.DateExpirationFrom, dateExpirationFrom.ToStringDateSortableInvariant()},
                                    { ParamsNames.DateExpirationTo,dateExpirationTo.ToStringDateSortableInvariant()},
                                }
                            ),
                });

            index++;
        }

        chartJsJsonModel.SliceLabelsToStepSearchLinks =
            new HtmlString(
                JsonSerializer.Serialize(
                    new ChartSliceLabelsToStepSearchLinksJsonModel() { Items = graphSliceToItemSearchList }
                    , WebJsonUtility.JsonObjectModelOptions
                    )
                );

        chartJsJsonModel.Data =
            BuildChartJsDataJsonConfig(
                sliceValueList
                , stepDescriptionList
                );

        return chartJsJsonModel;
    }


    #region private helping methods 

    private static IHtmlContent BuildChartJsDataJsonConfig(
        IList<string> chartSliceElementsCountList
        , IList<string> chartSliceLabelList
        )
    {
        Models.Data data =
            new()
            {
                Datasets =
                    new List<Dataset>()
                    {
                        new Dataset()
                        {
                            Data = chartSliceElementsCountList,
                            Labels = chartSliceLabelList,
                        }
                    },

                Labels = chartSliceLabelList,
                //labels object repeated also here to make plugins and slices-url work
            };

        return
            new HtmlString(
                JsonSerializer.Serialize(data, WebJsonUtility.JsonObjectModelOptions)
                );
    }
    #endregion
}