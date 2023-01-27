namespace Examples.AspNetMvcCode.Web.ViewComponents;

/// <summary>
/// this component handles back navigation logic for all pages;
/// only exception are the item views which have a specific component to 
/// handle back url
/// </summary>
public class BackUrlComp : ViewComponent
{
    private readonly ILogger<BackUrlComp> _logger;
    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;

    private readonly IMainLocalizer _localizer;

    public BackUrlComp(
        ILogger<BackUrlComp> logger
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IMainLocalizer localizer
        )
    {
        _logger = logger;
        _webHttpContextAccessor = webHttpContextAccessor;
        _localizer = localizer;
    }


    public async Task<IViewComponentResult> InvokeAsync(BackUrlConfigViewModel model)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(InvokeAsync) }
                });

        _logger.LogDebug("CALL");



        Guard.Against.Null(model, nameof(BackUrlConfigViewModel));


        string label;
        RouteViewModel routeModel;

        switch (model.BackUrlConfig)
        {
            case BackUrlConfig.TenantLogin:
                routeModel = _webHttpContextAccessor.GetRouteForBackTenantLogin(false);

                label = _localizer[nameof(LocalizedStr.SharedLabelBack)];
                break;


            //use this to generate link to return to ItemManagement
            case BackUrlConfig.PageFromItemManagement:

                routeModel =
                    new RouteViewModel()
                    {
                        Controller = MvcComponents.CtrlItemManagement,
                        Action = MvcComponents.ActViewAndManage,
                        QueryStringValues =
                            new Dictionary<string, string>
                            {
                                { ParamsNames.ItemId, _webHttpContextAccessor.SessionItemIdCurrentlyManagedByUser.ToString()}
                            },
                    };
                label = _localizer[nameof(LocalizedStr.SharedLabelBack)];
                break;


            case BackUrlConfig.InsertNew:

                routeModel =
                    new RouteViewModel()
                    {
                        Controller = MvcComponents.CtrlItemManagement,
                        Action = MvcComponents.ActStartNewItem,
                    };
                label = _localizer[nameof(LocalizedStr.SharedLabelBack)];
                break;


            case BackUrlConfig.AdminApp:
                routeModel =
                    new RouteViewModel()
                    {
                        Controller = MvcComponents.CtrlAdministration,
                        Action = MvcComponents.ActAvailableActions,
                    };
                label = _localizer[nameof(LocalizedStr.SharedLabelBack)];
                break;

            case BackUrlConfig.UserSupervisorSearch:
                routeModel = _webHttpContextAccessor.GetRouteForBackUserViewAndManage();
                label = _localizer[nameof(LocalizedStr.SharedLabelBack)];
                break;

            default:
                throw new PmWebException($"{typeof(BackUrlComp)}: invalid value '{model.BackUrlConfig}' ");
        }

        return
            await Task.FromResult<IViewComponentResult>(
                           View(
                               MvcComponents.SharedViewCompBackUrl,
                               new BackUrlViewModel()
                               {
                                   Route = routeModel,
                                   Label = label,
                               })
                           )
                        .ConfigureAwait(false);
    }
}