namespace Comunica.ProcessManager.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 2)]
public class ProcessiController : BaseContextController
{
    private readonly ContextUser _contextUser;
    private readonly IAuthorizationCustomWeb _webAuthorizationCustom;
    private readonly MainLocalizer _localizer;


    public ProcessiController(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , ContextUser contextUser
        , IAuthorizationCustomWeb webAuthorizationCustom
        , MainLocalizer localizer
        ) : base(httpContextAccessorCustomWeb)
    {
        _contextUser = contextUser;
        _webAuthorizationCustom = webAuthorizationCustom;
        _localizer = localizer;
    }



    [HttpGet]
    public IActionResult Benvenuto(
        [FromServices] IProcessLogic _logicProcess
        , [FromServices] IPersonalizationWeb _webPersonalization
        )
    {
        _httpContextAccessorCustomWeb.SessionIdItemCurrentlyManagedByUser = long.MinValue;

        //this check determine if it has sense to bypass starting page and go directly to new item compilation
        ProcessSelectionResultLgc result = _logicProcess.CheckIfOnlyOneProcessIsSelectable();

        if (!result.Success)
        {
            OperationResultViewModel modelMessage = result.MapProcessSelectionResult();
            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.ProcessUnauthorizedForInsert)];
            _httpContextAccessorCustomWeb.SessionOperationResult = modelMessage;

            _httpContextAccessorCustomWeb.SessionIdItemCurrentlyManagedByUser = long.MinValue;
            _httpContextAccessorCustomWeb.SessionProcessLogoFileName = string.Empty;
            _httpContextAccessorCustomWeb.SessionHasSingleProcessConfiguration = true;

            return RedirectToRoute(_webAuthorizationCustom.GetLandingPageByRole(null, null));
        }


        //success
        if (result.SelectedProcessId.Valid())
        {
            _httpContextAccessorCustomWeb.SessionProcessId = result.SelectedProcessId;
            _httpContextAccessorCustomWeb.SessionProcessLogoFileName = string.Empty;//for single process configuration logo change by process has no sense
            _httpContextAccessorCustomWeb.SessionHasSingleProcessConfiguration = true;
            _contextUser.ProcessId = result.SelectedProcessId;

            return RedirectToAction(
                MvcComponents.ActStartNewItem
                , MvcComponents.CtrlItemManagement
                );
        }


        //cleanup before start
        _httpContextAccessorCustomWeb.SessionProcessId = long.MinValue;
        _httpContextAccessorCustomWeb.SessionProcessLogoFileName = string.Empty;
        _contextUser.ProcessId = long.MinValue;

        LandingAndSelectionContentViewModel model = _webPersonalization.GetLandingAndSelectionContent();
        return View(MvcComponents.ViewLandingAndSelection, model);
    }



    [HttpPost]
    public IActionResult SelezionaProcesso(
        [FromServices] IProcessLogic _logicProcess
        , [FromServices] IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper
        , IFormCollection processSelectionForm
        )
    {
        ProcessSelectionResultModel model =
            _webHtmlFormToModelMapper.MapProcessSelectionFromPostedForm(processSelectionForm);

        //even no keys can be allowed if configuration allows it (vedi caredent)
        ProcessSelectionResultLgc processSelectionResult =
             _logicProcess.ValidateAndGet(
                 mainId: model.MainId
                 , autoCode: model.ReportAutoCode
                 , channelCode: model.ChannelCode
                 , logicGroupId: model.LogicGroupId
                 );

        if (processSelectionResult.Success)
        {
            _httpContextAccessorCustomWeb.SessionProcessId = processSelectionResult.SelectedProcessId;
            _httpContextAccessorCustomWeb.SessionProcessLogoFileName = processSelectionResult.SelectedProcessLogoFileName;
            _contextUser.ProcessId = processSelectionResult.SelectedProcessId;

            return RedirectToAction(
                    MvcComponents.ActStartNewItem,
                    MvcComponents.CtrlItemManagement
                    );
        }
        else
        {
            OperationResultViewModel modelMessage = processSelectionResult.MapProcessSelectionResult();
            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.ProcessSelectedUnauthorizedForInsert)];
            _httpContextAccessorCustomWeb.SessionOperationResult = modelMessage;

            _httpContextAccessorCustomWeb.SessionIdItemCurrentlyManagedByUser = long.MinValue;
            _httpContextAccessorCustomWeb.SessionProcessLogoFileName = string.Empty;
            _httpContextAccessorCustomWeb.SessionHasSingleProcessConfiguration = true;

            return RedirectToRoute(_webAuthorizationCustom.GetLandingPageByRole(null, null));
        }
    }



    //per ora gestiamo i processi subordinati come semplice link SENZA passare da modal/popup
    //come nei processi primari per mancanza di tempo
    //nel caso si presenta la necessità di gestirne più di uno, 
    //implementare riutilizzando le views e il codice per i processi primari
    [Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
    [ServiceFilter(typeof(RequiresManagedIdItemFilter), Order = 3)]
    [HttpPost]
    public IActionResult SelezionaProcessoCollegato(
        [FromServices] IItemLinkedLogic _logicItemLinked
        , long idProcessoCollegato
        , long idProcessoMaster
        , string faseMaster
        , string statoMaster
        )
    {
        ProcessSelectionResultLgc result = _logicItemLinked.ValidateProcessForOpenLinkedItem(
                processLinkedId: idProcessoCollegato
                , processMasterId: idProcessoMaster
                , phaseMaster: faseMaster
                , stateMaster: statoMaster
                );

        if (result.Success)
        {
            _httpContextAccessorCustomWeb.SessionProcessId = idProcessoCollegato;
            //logo non gestito per questa modalità al momento
            _httpContextAccessorCustomWeb.SessionProcessLogoFileName = string.Empty;
            _contextUser.ProcessId = idProcessoCollegato;

            return RedirectToAction(
                    MvcComponents.ActStartNewLinkedItem
                    , MvcComponents.CtrlItemManagement
                    );
        }
        else
        {
            OperationResultViewModel modelMessage = result.MapProcessSelectionResult();
            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.ProcessLinkedSelectedUnauthorizedForInsert)];
            _httpContextAccessorCustomWeb.SessionOperationResult = modelMessage;

            _httpContextAccessorCustomWeb.SessionProcessId = long.MinValue;
            _httpContextAccessorCustomWeb.SessionProcessLogoFileName = string.Empty;
            _contextUser.ProcessId = long.MinValue;

            return RedirectToAction(
                MvcComponents.ActViewAndManageCurrent
                , MvcComponents.CtrlItemManagement
                );
        }
    }
}
