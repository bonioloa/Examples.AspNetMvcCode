namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 2)]
public class ProcessiController : BaseContextController
{
    private readonly ContextUser _contextUser;
    private readonly IAuthorizationCustomWeb _webAuthorizationCustom;
    private readonly IMainLocalizer _localizer;


    public ProcessiController(
        IHttpContextAccessorWeb webHttpContextAccessor
        , ContextUser contextUser
        , IAuthorizationCustomWeb webAuthorizationCustom
        , IMainLocalizer localizer
        ) : base(webHttpContextAccessor)
    {
        _contextUser = contextUser;
        _webAuthorizationCustom = webAuthorizationCustom;
        _localizer = localizer;
    }



    [HttpGet]
    public IActionResult Benvenuto(
        [FromServices] IProcessSelectionLogic _logicProcessSelection
        , [FromServices] IPersonalizationWeb _webPersonalization
        )
    {
        _webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser = long.MinValue;

        //this check determine if it has sense to bypass starting page and go directly to new item compilation
        ProcessSelectionResultLgc result = _logicProcessSelection.CheckIfOnlyOneProcessIsSelectable();

        if (!result.Success)
        {
            OperationResultViewModel modelMessage = result.MapProcessSelectionResult();
            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.ProcessUnauthorizedForInsert)];

            _webHttpContextAccessor.SessionOperationResult = modelMessage;

            _webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser = long.MinValue;
            _webHttpContextAccessor.SessionProcessLogoFileName = string.Empty;
            _webHttpContextAccessor.SessionHasSingleProcessConfiguration = true;

            return
                RedirectToRoute(
                    _webAuthorizationCustom.GetLandingPageByRole(null, null)
                    );
        }


        //success
        if (result.SelectedProcessId.Valid())
        {
            _webHttpContextAccessor.SessionProcessId = result.SelectedProcessId;
            _webHttpContextAccessor.SessionProcessLogoFileName = string.Empty;//for single process configuration logo change by process has no sense
            _webHttpContextAccessor.SessionHasSingleProcessConfiguration = true;
            _contextUser.ProcessId = result.SelectedProcessId;

            return
                RedirectToAction(
                    MvcComponents.ActStartNewItem
                    , MvcComponents.CtrlItemManagement
                    );
        }


        //cleanup before start
        _webHttpContextAccessor.SessionProcessId = long.MinValue;
        _webHttpContextAccessor.SessionProcessLogoFileName = string.Empty;
        _contextUser.ProcessId = long.MinValue;

        return
            View(
                MvcComponents.ViewLandingAndSelection
                , _webPersonalization.GetLandingAndSelectionContent()
                );
    }



    [HttpPost]
    public IActionResult SelezionaProcesso(
        [FromServices] IProcessSelectionLogic _logicProcessSelection
        , [FromServices] IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper
        , IFormCollection processSelectionForm
        )
    {
        ProcessSelectionResultModel model =
            _webHtmlFormToModelMapper.MapProcessSelectionFromPostedForm(processSelectionForm);

        //even no keys can be allowed if configuration allows it (vedi caredent)
        ProcessSelectionResultLgc processSelectionResult =
             _logicProcessSelection.ValidateAndGet(
                 mainId: model.MainId
                 , autoCode: model.ReportAutoCode
                 , channelCode: model.ChannelCode
                 , logicGroupId: model.LogicGroupId
                 );

        if (processSelectionResult.Success)
        {
            _webHttpContextAccessor.SessionProcessId = processSelectionResult.SelectedProcessId;
            _webHttpContextAccessor.SessionProcessLogoFileName = processSelectionResult.SelectedProcessLogoFileName;

            _contextUser.ProcessId = processSelectionResult.SelectedProcessId;

            return
                RedirectToAction(
                    MvcComponents.ActStartNewItem,
                    MvcComponents.CtrlItemManagement
                    );
        }


        OperationResultViewModel modelMessage = processSelectionResult.MapProcessSelectionResult();
        modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.ProcessSelectedUnauthorizedForInsert)];

        _webHttpContextAccessor.SessionOperationResult = modelMessage;

        _webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser = long.MinValue;
        _webHttpContextAccessor.SessionProcessLogoFileName = string.Empty;
        _webHttpContextAccessor.SessionHasSingleProcessConfiguration = true;


        return
            RedirectToRoute(
                _webAuthorizationCustom.GetLandingPageByRole(null, null)
                );
    }



    //per ora gestiamo i processi subordinati come semplice link SENZA passare da modal/popup
    //come nei processi primari per mancanza di tempo
    //nel caso si presenta la necessità di gestirne più di uno, 
    //implementare riutilizzando le views e il codice per i processi primari
    [Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
    [ServiceFilter(typeof(RequiresManagedItemIdFilter), Order = 3)]
    [HttpPost]
    public IActionResult SelezionaProcessoCollegato(
        [FromServices] IItemLinkedLogic _logicItemLinked
        , long idProcessoCollegato
        , long idProcessoMaster
        , string faseMaster
        , string statoMaster
        )
    {
        ProcessSelectionResultLgc result =
            _logicItemLinked.ValidateProcessForOpenLinkedItem(
                processLinkedId: idProcessoCollegato
                , processMasterId: idProcessoMaster
                , phaseMaster: faseMaster
                , stateMaster: statoMaster
                );

        if (result.Success)
        {
            _webHttpContextAccessor.SessionProcessId = idProcessoCollegato;
            //logo non gestito per questa modalità al momento
            _webHttpContextAccessor.SessionProcessLogoFileName = string.Empty;
            _contextUser.ProcessId = idProcessoCollegato;

            return RedirectToAction(
                    MvcComponents.ActStartNewLinkedItem
                    , MvcComponents.CtrlItemManagement
                    );
        }


        OperationResultViewModel modelMessage = result.MapProcessSelectionResult();
        modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.ProcessLinkedSelectedUnauthorizedForInsert)];

        _webHttpContextAccessor.SessionOperationResult = modelMessage;

        _webHttpContextAccessor.SessionProcessId = long.MinValue;
        _webHttpContextAccessor.SessionProcessLogoFileName = string.Empty;
        _contextUser.ProcessId = long.MinValue;

        return
            RedirectToAction(
                MvcComponents.ActViewAndManageCurrent
                , MvcComponents.CtrlItemManagement
                );
    }
}