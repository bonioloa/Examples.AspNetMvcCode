namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class UserResetPasswordComp : ViewComponent
{
    private readonly ILogger<UserResetPasswordComp> _logger;

    public UserResetPasswordComp(
        ILogger<UserResetPasswordComp> logger
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



        if (model.IsActive && !model.UserMustBeIncludedInManagedBeforeEdits)
        {
            //show if user is active and already included in managed
            return
                await Task.FromResult<IViewComponentResult>(
                        View(MvcComponents.ViewCompUserResetPassword, model)
                        ).ConfigureAwait(false);
        }
        else
        {
            return
                await Task.FromResult<IViewComponentResult>(
                                    Content(string.Empty)
                                    ).ConfigureAwait(false);
        }
    }
}