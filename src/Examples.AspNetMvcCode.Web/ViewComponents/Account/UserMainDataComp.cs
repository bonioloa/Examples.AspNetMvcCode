namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class UserMainDataComp : ViewComponent
{
    private readonly ILogger<UserMainDataComp> _logger;

    public UserMainDataComp(
        ILogger<UserMainDataComp> logger
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



        if (!model.UserMustBeIncludedInManagedBeforeEdits && model.IsActive)
        {
            //necessary condition to show view in edit mode
            return
                await Task.FromResult<IViewComponentResult>(
                        View(MvcComponents.ViewCompUserEditData, model)
                        ).ConfigureAwait(false);
        }
        //in all other cases all fields are in read only mode.
        return
            await Task.FromResult<IViewComponentResult>(
                    View(MvcComponents.ViewCompUserDisplayData, model)
                    ).ConfigureAwait(false);
    }
}