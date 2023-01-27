namespace Comunica.ProcessManager.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
[ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 1)]
public class ReportisticaController : BaseContextController
{
    private readonly IReportLogic _logicReport;

    private readonly MainLocalizer _localizer;
    private readonly HtmlMainLocalizer _htmlLocalizer;
    private readonly IReportingDocumentWeb _webReportingDocument;
    private readonly IChartJsBuilderWeb _webChartJsBuilder;
    private readonly IPersonalizationWeb _webPersonalization;
    private readonly IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper;


    public ReportisticaController(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , IReportLogic logicReport
        , MainLocalizer localizer
        , HtmlMainLocalizer htmlLocalizer
        , IReportingDocumentWeb webReportingDocument
        , IChartJsBuilderWeb webChartJsBuilder
        , IPersonalizationWeb webPersonalization
        , IHtmlFormToModelMapperWeb webHtmlFormToModelMapper
        ) : base(httpContextAccessorCustomWeb)
    {
        _logicReport = logicReport;
        _localizer = localizer;
        _htmlLocalizer = htmlLocalizer;
        _webReportingDocument = webReportingDocument;
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
            , ReportExportDataModel reportModel
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


        FileDownloadInfoModel model =
            _webReportingDocument.BuildExcelReport(
                reportModel
                , getForAllAvailableProcesses
                , dateSubmitFrom: dataInvioDa.Clean()
                , dateSubmitTo: dataInvioA.Clean()
                , hasItemEndEditableField: modelWithFilters.EnableDateExpirationFilter
                , dateExpirationFromFilter: dataScadenzaDa.Clean()
                , dateExpirationToFilter: dataScadenzaA.Clean()
                );

        return
            File(
                fileContents: model.FileContents
                , contentType: model.ContentType
                , fileDownloadName: model.FileName
                );
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
           , ReportExportDataModel reportModel
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


        ReportAdvancedModel reportAdvancedModel =
           _webReportingDocument.BuildReportAdvanced(
                reportModel
                , modelWithFilters.EnableDateExpirationFilter
                );
        modelWithFilters.ColumnsOptionsList = reportAdvancedModel.OptionsForReport;
        modelWithFilters.DataSourceGrid = reportAdvancedModel.DataSourceGrid;
        modelWithFilters.TenantImage = reportAdvancedModel.TenantLogoToBase64;

        return View(MvcComponents.ViewReportAdvanced, modelWithFilters);
    }


    [NonAction]
    private (
        IActionResult action
        , bool getForAllAvailableProcesses
        , ReportExportDataModel reportModel
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
                , _localizer[nameof(LocalizedStr.SharedAttachmentFilesTitle)]
                , dateSubmitFromFilter: dateSubmitFromFilter.Clean()
                , dateSubmitToFilter: dateSubmitToFilter.Clean()
                , dateExpirationFromFilter: dateExpirationFromFilter.Clean()
                , dateExpirationToFilter: dateExpirationToFilter.Clean()
                , addLinkedItemsToResults
                );

        if (!reportFiltersAndAggregate.Success)
        {
            OperationResultViewModel modelMessage = reportFiltersAndAggregate.MapReportExportResult();
            _httpContextAccessorCustomWeb.SessionOperationResult = modelMessage;

            return (
                action:
                    RedirectToAction(
                        MvcComponents.ActReportingDataExport
                        , MvcComponents.CtrlReporting
                        )
                , getForAllAvailableProcesses
                , reportModel: null
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
                        }
                    )
                , getForAllAvailableProcesses
                , reportModel: null
                , modelWithFilters: null
                );
        }

        ReportExportViewModel modelWithFilters = reportFiltersAndAggregate.MapPartialFromLogicToViewModel();
        if (processFilterIsSingleChoice)
        {
            modelWithFilters.ProcessSelect = AddStaticLocalizedElements(modelWithFilters.ProcessSelect);
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
                , reportModel: null
                , modelWithFilters
                );
        }

        ReportExportDataModel reportExportDataModel = reportFiltersAndAggregate.MapFromLogicToWeb();

        return (
            action: null
            , getForAllAvailableProcesses
            , reportModel: reportExportDataModel
            , modelWithFilters
            );
    }


    [HttpPost]
    public IActionResult ReportAvanzatoSalvaLayout(IFormCollection form)
    {
        DataGridViewSaveViewModel gridViewSaveViewModel = 
            _webHtmlFormToModelMapper.MapDataGridViewSave(
                form
                , DataGridViewUsage.ReportAdvanced
                );

        return Json(
            new SubmitResultJsonModel
            {
                ResultCode = "OK",
                ErrorMessage = string.Empty,
            });
    }

    [HttpPost]
    public IActionResult ReportAvanzatoCaricaLayout(IFormCollection form)
    {
        return Json(
            new SubmitResultJsonModel
            {
                ResultCode = "OK",
                ErrorMessage = string.Empty,
            });
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
                }
                );
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
        ReportStatisticsViewModel reportViewModel = new()
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
            FieldsToWarn = objIn.FieldsToWarn,
            ValuesAllowed = objIn.ValuesAllowed
        };
        reportViewModel.ProcessSelect = AddStaticLocalizedElements(reportViewModel.ProcessSelect);
        return reportViewModel;
    }

    [NonAction]
    private IList<OptionViewModel> AddStaticLocalizedElements(IList<OptionViewModel> objIn)
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
}