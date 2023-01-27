namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
[ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 1)]
public class ReportisticaController : BaseContextController
{
    private readonly ILogger<ReportisticaController> _logger;

    private readonly IReportLogic _logicReport;
    private readonly IDataGridLogic _logicDataGrid;
    private readonly IReportingDocumentLogic _logicReportingDocument;

    private readonly IMainLocalizer _localizer;
    private readonly IHtmlMainLocalizer _htmlLocalizer;
    private readonly IChartJsBuilderWeb _webChartJsBuilder;
    private readonly IPersonalizationWeb _webPersonalization;
    private readonly IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper;


    public ReportisticaController(
        ILogger<ReportisticaController> logger
        , IReportLogic logicReport
        , IDataGridLogic logicDataGrid
        , IReportingDocumentLogic logicReportingDocument
        , IMainLocalizer localizer
        , IHtmlMainLocalizer htmlLocalizer
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IChartJsBuilderWeb webChartJsBuilder
        , IPersonalizationWeb webPersonalization
        , IHtmlFormToModelMapperWeb webHtmlFormToModelMapper
        ) : base(webHttpContextAccessor)
    {
        _logger = logger;
        _logicReport = logicReport;
        _logicDataGrid = logicDataGrid;
        _logicReportingDocument = logicReportingDocument;
        _localizer = localizer;
        _htmlLocalizer = htmlLocalizer;
        _webChartJsBuilder = webChartJsBuilder;
        _webPersonalization = webPersonalization;
        _webHtmlFormToModelMapper = webHtmlFormToModelMapper;
    }



    [HttpGet]
    public IActionResult ScaricaReport(
        string tuttiProcessi
        , IList<long> idProcessi
        , [ValidateAsDateStringFromQuery] string dataInvioDa
        , [ValidateAsDateStringFromQuery] string dataInvioA
        , [ValidateAsDateStringFromQuery] string dataScadenzaDa
        , [ValidateAsDateStringFromQuery] string dataScadenzaA
        )
    {
        (
            IActionResult action
            , bool getForAllAvailableProcesses
            , ReportExportDataLgc reportInfo
            , ReportExportViewModel modelWithFilters
            ) = GetFiltersAndReportDataInner(
                    allProcesses: tuttiProcessi
                    , processIdList: idProcessi
                    , processFilterIsSingleChoice: false
                    , dateSubmitFromFilter: dataInvioDa
                    , dateSubmitToFilter: dataInvioA
                    , dateExpirationFromFilter: dataScadenzaDa
                    , dateExpirationToFilter: dataScadenzaA
                    , addLinkedItemsToResultsFilter: AppConstants.OptionAffirmativeValue //linked always included for normal report
                    , pageTitle: _localizer[nameof(LocalizedStr.ReportPageTitle)].ToString()
                    , viewName: MvcComponents.ViewReportDataExport
                    );

        if (action != null)
        {
            return action;
        }


        FileDownloadInfoLgc fileDownloadInfo =
            _logicReportingDocument.BuildExcelReport(
                reportDataDump: reportInfo
                , getForAllAvailableProcesses
                , dateSubmitFrom: dataInvioDa.Clean()
                , dateSubmitTo: dataInvioA.Clean()
                , hasItemEndEditableField: modelWithFilters.EnableDateExpirationFilter
                , dateExpirationFromFilter: dataScadenzaDa.Clean()
                , dateExpirationToFilter: dataScadenzaA.Clean()
                );

        return base.BuildFileResult(fileDownloadInfo);
    }



    [HttpGet]
    public IActionResult ReportAvanzato(
        long? idProcesso
        , [ValidateAsDateStringFromQuery] string dataInvioDa
        , [ValidateAsDateStringFromQuery] string dataInvioA
        , [ValidateAsDateStringFromQuery] string dataScadenzaDa
        , [ValidateAsDateStringFromQuery] string dataScadenzaA
        , [ValidateAsLiteralStringFromQuery] string recuperaAncheCollegatiAdElementi
        )
    {
        (
           IActionResult action
           , _
           , ReportExportDataLgc reportInfo
           , ReportExportViewModel modelWithFilters
           ) = GetFiltersAndReportDataInner(
                   allProcesses: string.Empty
                   , processIdList: idProcesso is null ? null : new List<long> { (long)idProcesso }
                   , processFilterIsSingleChoice: true
                   , dateSubmitFromFilter: dataInvioDa
                   , dateSubmitToFilter: dataInvioA
                   , dateExpirationFromFilter: dataScadenzaDa
                   , dateExpirationToFilter: dataScadenzaA
                   , addLinkedItemsToResultsFilter: recuperaAncheCollegatiAdElementi
                   , pageTitle: _localizer[nameof(LocalizedStr.ReportAdvancedPageTitle)].ToString()
                   , viewName: MvcComponents.ViewReportAdvanced
                   );

        if (action != null)
        {
            return action;
        }


        ReportAdvancedLgc reportAdvancedModel =
           _logicReportingDocument.BuildReportAdvanced(
                reportInfo
                , modelWithFilters.EnableDateExpirationFilter
                );


        modelWithFilters.ColumnsOptionsList = reportAdvancedModel.OptionsForReport.MapHashSetFromLogicToWeb();
        modelWithFilters.DataSourceGrid = reportAdvancedModel.DataSourceGrid;
        modelWithFilters.TenantImage = reportAdvancedModel.TenantLogoToBase64;

        return
            View(
                MvcComponents.ViewReportAdvanced
                , modelWithFilters
                );
    }


    [NonAction]
    private (
        IActionResult action
        , bool getForAllAvailableProcesses
        , ReportExportDataLgc reportInfo
        , ReportExportViewModel modelWithFilters
        )
        GetFiltersAndReportDataInner(
            string allProcesses
            , IList<long> processIdList
            , bool processFilterIsSingleChoice
            , string dateSubmitFromFilter
            , string dateSubmitToFilter
            , string dateExpirationFromFilter
            , string dateExpirationToFilter
            , string addLinkedItemsToResultsFilter
            , string pageTitle
            , string viewName
            )
    {
        bool getForAllAvailableProcesses =
            allProcesses.StringHasValue()
                && allProcesses == AppConstants.FilterDefaultValueAllAvailableProcesses.ToString();


        bool addLinkedItemsToResults = false;
        if (addLinkedItemsToResultsFilter is null)
        {
            addLinkedItemsToResults = true;
        }

        if (addLinkedItemsToResultsFilter.StringHasValue()
            && addLinkedItemsToResultsFilter.EqualsInvariant(AppConstants.OptionAffirmativeValue))
        {
            addLinkedItemsToResults = true;
        }


        ReportExportDataLgc reportFiltersAndAggregate =
            _logicReport.HandleFiltersAndAggregateData(
                allProcesses: getForAllAvailableProcesses
                , processIdList: processIdList
                , dateSubmitFromFilter: dateSubmitFromFilter.Clean()
                , dateSubmitToFilter: dateSubmitToFilter.Clean()
                , dateExpirationFromFilter: dateExpirationFromFilter.Clean()
                , dateExpirationToFilter: dateExpirationToFilter.Clean()
                , addLinkedItemsToResults
                );


        if (!reportFiltersAndAggregate.Success)
        {
            _webHttpContextAccessor.SessionOperationResult =
                reportFiltersAndAggregate.MapReportExportResult();

            return (
                action:
                    RedirectToAction(
                        MvcComponents.ActReportingDataExport
                        , MvcComponents.CtrlReporting
                        )
                , getForAllAvailableProcesses
                , reportInfo: null
                , modelWithFilters: null
                );
        }


        if (reportFiltersAndAggregate.NoItemsForRoleFound)
        {
            return (
                 action:
                     View(
                        MvcComponents.ViewEmptyReport,
                        new ReportEmptyViewModel()
                        {
                            PageTitle = pageTitle,
                            NoItemsMessage = _webPersonalization.GetMessageNoItemsReport(),
                        })
                , getForAllAvailableProcesses
                , reportInfo: null
                , modelWithFilters: null
                );
        }

        ReportExportViewModel modelWithFilters = reportFiltersAndAggregate.MapPartialFromLogicToViewModel();

        if (processFilterIsSingleChoice)
        {
            modelWithFilters.ProcessSelect = AddDefaultOption(modelWithFilters.ProcessSelect);
        }


        if (reportFiltersAndAggregate.FilterPreloadCall
            || reportFiltersAndAggregate.ShowMessageNoResultsFound)
        {
            return (
                action:
                    View(
                        viewName
                        , modelWithFilters
                        )
                , getForAllAvailableProcesses
                , reportInfo: null
                , modelWithFilters
                );
        }


        long singleSelectedProcessId = 0;
        if (modelWithFilters.ProcessSelect.Any(o => o.Selected))
        {
            singleSelectedProcessId =
                modelWithFilters.ProcessSelect.Where(o => o.Selected)
                                              .Select(o => long.Parse(o.Value))
                                              .First();
        }


        //no preselected values here, js will assign default view selection element, not existing to db
        //Get first selected process, because this object will have meaning
        //only with single selection filter, so it's ok to take first
        modelWithFilters.AvailableGridStateForLoadList =
            MapAndAddDefaultChoice(
                _logicDataGrid.GetSavedViews(
                    DataGridViewUsage.ReportAdvanced
                    , singleSelectedProcessId
                    , null
                    )
                );

        return (
            action: null
            , getForAllAvailableProcesses
            , reportInfo: reportFiltersAndAggregate
            , modelWithFilters
            );
    }


    [HttpPost]
    public IActionResult ReportAvanzatoSalvaLayout(IFormCollection form)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ReportAvanzatoSalvaLayout) }
                });



        DataGridStateViewSaveResultJsonModel model = new();

        try//wrap everything under try because error page will not be shown with an ajax post
        {
            DataGridViewStateSaveLgc dataGridViewStateSaveLgc =
                _webHtmlFormToModelMapper
                    .MapDataGridViewStateSave(
                        form
                        , DataGridViewUsage.ReportAdvanced
                        )
                    .MapFromWebToLogic();

            long newDataGridStateId = _logicDataGrid.SaveView(dataGridViewStateSaveLgc);

            /*WARNING: la transazione SQL che salva la view non è completa su db prima del recupero liste vista
             questo comando ferma l'esecuzione dell' app per date tempo al db a completare comando*/
            Thread.Sleep(1 * 1000);

            //reload saved values to rebuild selection
            model.SavedDataGridViewStateList =
                MapAndAddDefaultChoice(
                    _logicDataGrid.GetSavedViews(
                        DataGridViewUsage.ReportAdvanced
                        , dataGridViewStateSaveLgc.ProcessId
                        , newDataGridStateId
                        )
                    );

            model.ResultCode = SharedResultCode.Ok.ToString();
            model.ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DataGrid state layout saving");

            model.ResultCode = SharedResultCode.Ko.ToString();
            model.ErrorMessage = _localizer[nameof(LocalizedStr.DataGridViewStateSaveError)];
        }

        return Json(model);
    }


    /// <summary>
    /// this action will be called by ajax post.
    /// Loads saved view 
    /// </summary>
    /// <param name="caricaStatoGrigliaSelect"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult ReportAvanzatoCaricaLayout(
        long idProcesso
        , long caricaStatoGrigliaSelect
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(ReportAvanzatoCaricaLayout) }
                });



        DataGridStateViewLoadResultJsonModel model = new();

        try//wrap everything under try because error page will not be shown with an ajax post
        {
            DataGridViewStateLgc dataGridViewStateLgc =
                _logicDataGrid.LoadView(
                    dataGridViewStateId: caricaStatoGrigliaSelect
                    , processId: idProcesso
                    , DataGridViewUsage.ReportAdvanced
                    );

            model.DataGridViewState = dataGridViewStateLgc.MapFromLogicToWeb();
            model.ResultCode = SharedResultCode.Ok.ToString();
            model.ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DataGrid state layout loading");

            model.ResultCode = SharedResultCode.Ko.ToString();
            model.ErrorMessage = _localizer[nameof(LocalizedStr.DataGridViewStateLoadError)];
        }

        return Json(model);
    }


    [HttpGet]
    public IActionResult Statistiche(
        long? idProcesso
        , [ValidateAsDateStringFromQuery] string dataInvioDa
        , [ValidateAsDateStringFromQuery] string dataInvioA
        , [ValidateAsDateStringFromQuery] string dataScadenzaDa
        , [ValidateAsDateStringFromQuery] string dataScadenzaA
        )
    {
        ReportStatisticsLgc reportItem =
            _logicReport.GetStatistics(
                processId:
                    idProcesso is null
                        ? AppConstants.FilterDefaultValueAllAvailableProcesses
                        : (long)idProcesso
                , dateSubmitFromFilter: dataInvioDa.Clean()
                , dateSubmitToFilter: dataInvioA.Clean()
                , dateExpirationFromFilter: dataScadenzaDa.Clean()
                , dateExpirationToFilter: dataScadenzaA.Clean()
                );

        if (reportItem.NoItemsForRoleFound)
        {
            return View(
                MvcComponents.ViewEmptyReport,
                new ReportEmptyViewModel()
                {
                    PageTitle = _localizer[nameof(LocalizedStr.ReportStatisticsPageTitle)],
                    NoItemsMessage = _webPersonalization.GetMessageNoItemsReport(),
                });
        }

        ReportStatisticsViewModel model = MapPartial(reportItem);

        if (reportItem.ShowStatistics && reportItem.StepStatistics.HasValues())
        {
            model.JsonStatisticsChart =
                _webChartJsBuilder.BuildReportStatisticsModel(
                    reportItem.StepStatistics.MapIListFromLogicToWeb()
                    , reportItem.ProcessId
                    , reportItem.DateSubmitFrom
                    , reportItem.DateSubmitTo
                    , reportItem.DateExpirationFrom
                    , reportItem.DateExpirationTo
                    );
        }

        return View(MvcComponents.ViewReportStatistics, model);
    }



    [NonAction]
    private ReportStatisticsViewModel MapPartial(ReportStatisticsLgc objIn)
    {
        ReportStatisticsViewModel reportViewModel =
            new()
            {
                ProcessSelect = objIn.ProcessSelect.MapIListFromLogicToWeb(),

                DateSubmitFrom = objIn.DateSubmitFrom,
                DateSubmitTo = objIn.DateSubmitTo,

                EnableDateExpirationFilter = objIn.EnableDateExpirationFilter,
                DateExpirationFrom = objIn.DateExpirationFrom,
                DateExpirationTo = objIn.DateExpirationTo,

                ShowStatisticsChart = objIn.ShowStatistics,

                Success = objIn.Success,
                WarningType = objIn.WarningType,
                FieldToWarnList = objIn.FieldToWarnList,
                ValuesAllowed = objIn.ValuesAllowed
            };

        reportViewModel.ProcessSelect = AddDefaultOption(reportViewModel.ProcessSelect);

        return reportViewModel;
    }

    [NonAction]
    private IList<OptionViewModel> AddDefaultOption(IList<OptionViewModel> objIn)
    {
        objIn.Insert(
            0,
            new OptionViewModel
            {
                Value = AppConstants.FilterDefaultValueAllAvailableProcesses.ToString(),
                Description = _htmlLocalizer[nameof(HtmlLocalization.SharedDefaultInvalidSelectionDescription)],
                Selected = !objIn.Any(p => p.Selected),
                Disabled = true,
            });

        return objIn;
    }

    [NonAction]
    private List<OptionViewModel> MapAndAddDefaultChoice(HashSet<OptionLgc> optionSet)
    {
        //default choice resets datagrid view, so it must be selectable and enabled
        List<OptionViewModel> savedViewsOptionList =
            optionSet.Select(o => o.MapFromLogicToWeb())
                     .OrderBy(o => long.Parse(o.Value))
                     .ToList();

        savedViewsOptionList.Insert(
            0,
            new OptionViewModel
            {
                Value = AppConstants.DataGridSelectDefaultViewCode,
                Description = _htmlLocalizer[nameof(HtmlLocalization.DataGridViewStateSelectDefaultOption)],
                Selected = !savedViewsOptionList.Any(o => o.Selected == true),
            });

        return savedViewsOptionList;
    }
}