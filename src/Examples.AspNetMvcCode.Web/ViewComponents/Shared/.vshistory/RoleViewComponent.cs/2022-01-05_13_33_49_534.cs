namespace Comunica.ProcessManager.Web.ViewComponents;

public class RoleViewComponent : ViewComponent
{
    private readonly ILogger<RoleViewComponent> _logger;
    private readonly ContextUser _contextUser;

    private readonly IAuthorizationCustomWeb _webAuthorizationCustom;


    public RoleViewComponent(
        ILogger<RoleViewComponent> logger
        , ContextUser contextUser
        , IAuthorizationCustomWeb webAuthorizationCustom
        )
    {
        _logger = logger;
        _contextUser = contextUser;
        _webAuthorizationCustom = webAuthorizationCustom;
    }




    public async Task<IViewComponentResult> InvokeAsync()
    {
        _logger.LogAppDebug("CALL");

        if (_contextUser.SupervisorRolesList.IsNullOrEmpty()
            || await _webAuthorizationCustom.UserIsNotLoggedOrIsNotSupervisorAsync().ConfigureAwait(false))
        {
            //don't show component content if not authorized or basic user
            return await Task.FromResult<IViewComponentResult>(
                Content(string.Empty)
                ).ConfigureAwait(false);
        }

        if (_contextUser.SupervisorRolesList.Count == 1)
        {
            return await Task.FromResult<IViewComponentResult>(
                View(MvcComponents.SharedViewCompRoleSingle, _contextUser.SupervisorRolesList[0].Description)
                ).ConfigureAwait(false);
        }

        IList<IHtmlContent> profileDescriptions =
            _contextUser.SupervisorRolesList
                        .Select(pd => pd.Description)
                        .ToList();

        return await Task.FromResult<IViewComponentResult>(
               View(
                   MvcComponents.SharedViewCompRoleMultiple
                   , profileDescriptions
                   )
               ).ConfigureAwait(false);
    }
}
