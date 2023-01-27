namespace Comunica.ProcessManager.Web.ViewComponents;

public class HeaderSecureAreaViewComponent : ViewComponent
{
    private readonly ILogger<HeaderSecureAreaViewComponent> _logger;
    private readonly ContextTenant _contextTenant;

    private readonly IPersonalizationLogic _logicPersonalization;

    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;

    public HeaderSecureAreaViewComponent(
        ILogger<HeaderSecureAreaViewComponent> logger
        , ContextTenant contextTenant
        , IPersonalizationLogic logicPersonalization
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        )
    {
        _logger = logger;
        _contextTenant = contextTenant;
        _logicPersonalization = logicPersonalization;
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
        _logger.LogAppDebug("CALL");

        if (!_httpContextAccessorCustomWeb.HttpContext.User.Identity.IsAuthenticated
            || _contextTenant is null || _contextTenant.LogoFileName.Empty())
        {
            //don't show component if user is anonymous
            return await Task.FromResult<IViewComponentResult>(
                Content(string.Empty)
                ).ConfigureAwait(false);
        }

        CompanyGroupInfoLgc companyGroupInfo = _logicPersonalization.GetInfo();
        HeaderSecureAreaViewModel headerModel = new()
        {
            TenantLogoFileName =
                _httpContextAccessorCustomWeb.SessionProcessLogoFileName.StringHasValue()
                ? _httpContextAccessorCustomWeb.SessionProcessLogoFileName
                : _contextTenant.LogoFileName,
            TenantWebsite = companyGroupInfo.WebsiteLink,
        };

        return await Task.FromResult<IViewComponentResult>(
            View(MvcComponents.SharedViewCompHeaderSecureArea, headerModel)
            ).ConfigureAwait(false);
    }
}
