namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class RoleDisplayComp : ViewComponent
{
    private readonly ILogger<RoleDisplayComp> _logger;
    private readonly ContextUser _contextUser;

    private readonly IAuthorizationCustomWeb _webAuthorizationCustom;

    private readonly IUserConfiguratorLogic _logicUserConfigurator;


    public RoleDisplayComp(
        ILogger<RoleDisplayComp> logger
        , ContextUser contextUser
        , IAuthorizationCustomWeb webAuthorizationCustom
        , IUserConfiguratorLogic logicUserConfigurator
        )
    {
        _logger = logger;
        _contextUser = contextUser;
        _webAuthorizationCustom = webAuthorizationCustom;
        _logicUserConfigurator = logicUserConfigurator;
    }




    public async Task<IViewComponentResult> InvokeAsync()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(InvokeAsync) }
                });

        _logger.LogDebug("CALL");


        if (_contextUser.AssignedSupervisorRolesFound.IsNullOrEmpty()
            || await _webAuthorizationCustom.UserIsNotLoggedOrIsNotSupervisorAsync().ConfigureAwait(false))
        {
            //don't show component content if not authorized or basic user
            return
                await Task.FromResult<IViewComponentResult>(
                    Content(string.Empty)
                    ).ConfigureAwait(false);
        }

        List<IHtmlContent> roleDescriptionList = _logicUserConfigurator.BuildRolesDisplayList();

        if (roleDescriptionList.Count == 1)
        {
            return
                await Task.FromResult<IViewComponentResult>(
                    View(MvcComponents.SharedViewCompRoleSingle, roleDescriptionList.First())
                    ).ConfigureAwait(false);
        }

        return
            await Task.FromResult<IViewComponentResult>(
               View(
                   MvcComponents.SharedViewCompRoleMultiple
                   , roleDescriptionList
                   )
               ).ConfigureAwait(false);
    }
}