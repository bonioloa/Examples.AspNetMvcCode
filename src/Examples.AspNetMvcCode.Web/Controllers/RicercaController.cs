namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserAccessedWithLoginAndPassword)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 1)]
public class RicercaController : BaseContextController
{
    private readonly IItemSearchLogic _logicItemSearch;

    private readonly IMainLocalizer _localizer;
    private readonly IDataTablesNetBuilderWeb _webDataTablesNetBuilder;
    private readonly IPersonalizationWeb _webPersonalization;

    public RicercaController(
        IItemSearchLogic logicItemSearch
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IMainLocalizer localizer
        , IDataTablesNetBuilderWeb webDataTablesNetBuilder
        , IPersonalizationWeb webPersonalization
        ) : base(webHttpContextAccessor)
    {
        _logicItemSearch = logicItemSearch;
        _localizer = localizer;
        _webDataTablesNetBuilder = webDataTablesNetBuilder;
        _webPersonalization = webPersonalization;
    }




    /// <summary>
    /// when all filters are empty we show page with only filters and without results
    /// </summary>
    /// <param name="idProcesso">always provided, 0 means all allowed processes</param>
    /// <param name="tipologiaStato">optional</param>
    /// <param name="stepProcesso">optional</param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult NuovaRicerca(
        long? idProcesso
        , [ValidateAsStringSimpleFromQuery] string tipologiaStato
        , [ValidateAsStringSimpleFromQuery] string stepProcesso
        , [ValidateAsDateStringFromQuery] string dataInvioDa
        , [ValidateAsDateStringFromQuery] string dataInvioA
        , [ValidateAsDateStringFromQuery] string dataScadenzaDa
        , [ValidateAsDateStringFromQuery] string dataScadenzaA
        )
    {
        ItemSearchFiltersLgc itemSearchFilters =
            new(
                ProcessId:
                     idProcesso is null
                     ? AppConstants.FilterDefaultValueAllAvailableProcesses
                     : (long)idProcesso
                , StepStateGroup: tipologiaStato.Clean()
                , Step: stepProcesso.Clean()
                , DateSubmitFrom: dataInvioDa.Clean()
                , DateSubmitTo: dataInvioA.Clean()
                , DateExpirationFrom: dataScadenzaDa.Clean()
                , DateExpirationTo: dataScadenzaA.Clean()
                );

        var stopwatch = new OperationTimingLogger(nameof(IItemSearchLogic.BuildAndReturnResults));
        ItemSearchResultLgc itemSearchResult =
            _logicItemSearch.BuildAndReturnResults(
                itemSearchFilters
                );
        stopwatch.LogCompletion();

        if (!itemSearchResult.Success)
        {
            OperationResultViewModel modelMessage = itemSearchResult.MapSearchOperationResult();
            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.SearchErrorMessage)];

            _webHttpContextAccessor.SessionOperationResult = modelMessage;

            return
                RedirectToAction(
                    MvcComponents.ActSearchNew
                    , MvcComponents.CtrlSearch
                    );
        }

        if (itemSearchResult.NoResultsForCurrentUser)
        {
            return View(MvcComponents.ViewEmptySearch, _webPersonalization.GetMessageNoItemsSearch());
        }

        _webHttpContextAccessor.SaveRouteForBackItemViewAndManage();


        ItemSearchResultViewModel model = MapPartialItemSearchResult(itemSearchResult);

        if (!model.ShowResults)
        {
            return View(MvcComponents.ViewSearch, model);
        }

        model.SearchResultsModel =
            _webDataTablesNetBuilder.BuildItemSearchResultModel(
                itemFoundList: itemSearchResult.FoundResults.MapIListFromLogicToWeb()
                , hasItemEndEditableField: itemSearchResult.EnableDateExpirationFilter
                );

        return View(MvcComponents.ViewSearch, model);
    }


    /// <summary>
    /// we need to use logic here because of localization required for
    /// static selection elements in filters
    /// </summary>
    /// <param name="model"></param>
    [NonAction]
    private ItemSearchResultViewModel AddStaticLocalizedElements(ItemSearchResultViewModel model)
    {
        IHtmlContent defaultProcessFilterText =
            new HtmlString(_localizer[nameof(LocalizedStr.SearchFilterProcessValueAll)]);

        //add default process filter element as first
        model.ProcessSelect.Insert(0,
            new OptionViewModel
            {
                Value = AppConstants.FilterDefaultValueAllAvailableProcesses.ToString(),
                Description = defaultProcessFilterText,
                Selected = !model.FilterOnProcesses
            });


        #region Select step state
        IHtmlContent defaultStepStateGroupFilterText =
            new HtmlString(_localizer[nameof(LocalizedStr.SharedStepStateGroupAll)]);

        model.StepStateGroupSelect =
            new List<OptionViewModel>()
            {
                new OptionViewModel()
                {
                    Value = StepStateGroupType.All.ToString(),
                    Description = defaultStepStateGroupFilterText,
                    Selected = model.SelectedStepStateGroup == StepStateGroupType.All
                },

                new OptionViewModel()
                {
                    Value = StepStateGroupType.Open.ToString(),
                    Description = new HtmlString(_localizer[nameof(LocalizedStr.SharedStepStateGroupOpen)]),
                    Selected = model.SelectedStepStateGroup == StepStateGroupType.Open
                },

                new OptionViewModel()
                {
                    Value = StepStateGroupType.Closed.ToString(),
                    Description = new HtmlString(_localizer[nameof(LocalizedStr.SharedStepStateGroupClosed)]),
                    Selected = model.SelectedStepStateGroup == StepStateGroupType.Closed
                }
            };

        if (model.HasAbortedStateGroup)
        {
            model.StepStateGroupSelect.Add(
                new OptionViewModel()
                {
                    Value = StepStateGroupType.Aborted.ToString(),
                    Description = new HtmlString(_localizer[nameof(LocalizedStr.SharedStepStateGroupAborted)]),
                    Selected = model.SelectedStepStateGroup == StepStateGroupType.Aborted,
                });
        }
        #endregion

        //add default step filter element as first
        model.StepSelect.Insert(0,
            new ProcessStepGroupFilterViewModel()
            {
                ProcessId = long.MaxValue,
                ProcessDescription = defaultProcessFilterText,
                Steps =
                    new List<ProcessStepFilterViewModel>()
                    {
                            new ProcessStepFilterViewModel()
                            {
                                Description = defaultStepStateGroupFilterText,
                                Code = AppConstants.FilterDefaultValueProcessStateAll,
                                Selected = !model.FilterOnStep,
                                StepStateGroup = StepStateGroupType.All,
                            }
                    },
            });

        return model;
    }
}