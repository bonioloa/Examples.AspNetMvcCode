namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class UserTenantManagedIncludeComp : ViewComponent
{
    private readonly ILogger<UserTenantManagedIncludeComp> _logger;

    public UserTenantManagedIncludeComp(
        ILogger<UserTenantManagedIncludeComp> logger
        )
    {
        _logger = logger;
    }


    public async Task<IViewComponentResult> InvokeAsync(UserEditSupervisorViewModel model)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(InvokeAsync) }
                });


        //show command only if user need inclusion
        if (model.UserMustBeIncludedInManagedBeforeEdits)
        {
            return
                await Task.FromResult<IViewComponentResult>(
                       View(MvcComponents.ViewCompUserTenantManagedInclude, model)
                       ).ConfigureAwait(false);
        }

        return
            await Task.FromResult<IViewComponentResult>(
                                Content(string.Empty)
                                ).ConfigureAwait(false);

    }
}