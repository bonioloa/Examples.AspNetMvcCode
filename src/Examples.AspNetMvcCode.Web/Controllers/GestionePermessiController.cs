namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserIsSupervisor)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 1)]
[ServiceFilter(typeof(RedirectIfAccessSimpleAnonymousFilter), Order = 3)]
[ServiceFilter(typeof(RequiresManagedItemIdFilter), Order = 4)]
public class GestionePermessiController : BaseContextController
{
    private readonly ContextApp _contextApp;
    private readonly ContextUser _contextUser;

    private readonly IItemPermissionsLogic _logicItemPermissions;
    private readonly IEmailSendAppNotificationLogic _logicEmailSendAppNotification;

    private readonly IHtmlFormToModelMapperWeb _webHtmlFormToModelMapper;
    private readonly IResultMessageMapperWeb _webResultMessageMapper;

    public GestionePermessiController(
        ContextApp contextApp
        , ContextUser contextUser
        , IItemPermissionsLogic logicItemPermissions
        , IEmailSendAppNotificationLogic logicEmailSendAppNotification
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IHtmlFormToModelMapperWeb webHtmlFormToModelMapper
        , IResultMessageMapperWeb webResultMessageMapper
        ) : base(webHttpContextAccessor)
    {
        _contextApp = contextApp;
        _contextUser = contextUser;
        _logicItemPermissions = logicItemPermissions;
        _logicEmailSendAppNotification = logicEmailSendAppNotification;
        _webHtmlFormToModelMapper = webHtmlFormToModelMapper;
        _webResultMessageMapper = webResultMessageMapper;
    }



    [HttpPost]
    public IActionResult AssegnaPermessi(
        IFormCollection formRoleInclusion
        )
    {
        RoleInclusionViewModel roleInclusion =
            _webHtmlFormToModelMapper.MapRoleInclusion(formRoleInclusion);

        RoleInclusionResultLgc result =
            _logicItemPermissions.ValidateAndAssignPermissions(
                cultureIsoCode: _contextApp.CurrentCultureIsoCode
                , userId: _contextUser.UserIdLoggedIn
                , userSupervisorRolesFound: _contextUser.AssignedSupervisorRolesFound
                , itemId: _contextUser.ItemIdCurrentlyManagedByUser
                , roleInclusion.MapFromWebToLogic()
                , assignmentDateTime: DateTime.Now
                );

        IList<OptionViewModel> includedRoles = result.IncludedRoles.MapIListFromLogicToWeb();
        OperationResultViewModel modelMessage = result.MapPermissionInclusionResult();

        _webHttpContextAccessor.SessionOperationResult =
            _webResultMessageMapper.SetRoleInclusionMessage(modelMessage, includedRoles);


        if (result.Success)
        {
            _logicEmailSendAppNotification.SendEmailNotificationInclusion();
        }


        //reload current item confirmation message
        return
            RedirectToAction(
                MvcComponents.ActViewAndManage
                , MvcComponents.CtrlItemManagement
                , new Dictionary<string, string>()
                    {
                        {ParamsNames.ItemId, _contextUser.ItemIdCurrentlyManagedByUser.ToString() }
                    }
                );
    }
}