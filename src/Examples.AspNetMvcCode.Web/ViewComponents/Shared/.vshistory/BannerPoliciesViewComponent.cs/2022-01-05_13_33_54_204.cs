namespace Comunica.ProcessManager.Web.ViewComponents;

public class BannerPoliciesViewComponent : ViewComponent
{
    private readonly ILogger<BannerPoliciesViewComponent> _logger;

    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;

    public BannerPoliciesViewComponent(
        ILogger<BannerPoliciesViewComponent> logger
        , IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , IOptionsSnapshot<ProductSettings> optProduct
       )
    {
        _logger = logger;
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
        _optProduct = optProduct;
    }



    public async Task<IViewComponentResult> InvokeAsync()
    {
        _logger.LogAppDebug("CALL");

        if (_optProduct.Value.DisablePrivacyComponents)
        {
            return await Task.FromResult<IViewComponentResult>(
                   Content(string.Empty)
                   ).ConfigureAwait(false);
        }

        return
            _httpContextAccessorCustomWeb.IsBannerCookiePolicyDismissed()

            ? await Task.FromResult<IViewComponentResult>(
                   Content(string.Empty)
                   ).ConfigureAwait(false)

            : await Task.FromResult<IViewComponentResult>(
                    View(MvcComponents.SharedViewCompBannerPolicies)
                    ).ConfigureAwait(false);
    }
}
