namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class HeaderSecureAreaComp : ViewComponent
{
    private readonly ILogger<HeaderSecureAreaComp> _logger;
    private readonly ContextTenant _contextTenant;

    private readonly IPersonalizationLogic _logicPersonalization;

    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;

    public HeaderSecureAreaComp(
        ILogger<HeaderSecureAreaComp> logger
        , ContextTenant contextTenant
        , IPersonalizationLogic logicPersonalization
        , IHttpContextAccessorWeb webHttpContextAccessor
        )
    {
        _logger = logger;
        _contextTenant = contextTenant;
        _logicPersonalization = logicPersonalization;
        _webHttpContextAccessor = webHttpContextAccessor;
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



        if (!_webHttpContextAccessor.HttpContext.User.Identity.IsAuthenticated
            || _contextTenant is null || _contextTenant.LogoFileName.Empty())
        {
            //don't show component if user is anonymous
            return
                await Task.FromResult<IViewComponentResult>(
                        Content(string.Empty)
                        ).ConfigureAwait(false);
        }


        CompanyGroupInfoLgc companyGroupInfo = _logicPersonalization.GetInfo();

        HeaderSecureAreaViewModel headerModel =
            new()
            {
                TenantLogoFileName =
                    _webHttpContextAccessor.SessionProcessLogoFileName.StringHasValue()
                    ? _webHttpContextAccessor.SessionProcessLogoFileName
                    : _contextTenant.LogoFileName,

                TenantWebsite = companyGroupInfo.WebsiteLink,
            };


        return
            await Task.FromResult<IViewComponentResult>(
                    View(MvcComponents.SharedViewCompHeaderSecureArea, headerModel)
                    ).ConfigureAwait(false);
    }
}