namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class BannerPoliciesComp : ViewComponent
{
    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;

    public BannerPoliciesComp(
        IHttpContextAccessorWeb webHttpContextAccessor
        , IOptionsSnapshot<ProductSettings> optProduct
       )
    {
        _webHttpContextAccessor = webHttpContextAccessor;
        _optProduct = optProduct;
    }



    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (_optProduct.Value.DisablePrivacyComponents)
        {
            return await Task.FromResult<IViewComponentResult>(
                   Content(string.Empty)
                   ).ConfigureAwait(false);
        }

        return
            _webHttpContextAccessor.IsBannerCookiePolicyDismissed()

            ? await Task.FromResult<IViewComponentResult>(
                   Content(string.Empty)
                   ).ConfigureAwait(false)

            : await Task.FromResult<IViewComponentResult>(
                    View(MvcComponents.SharedViewCompBannerPolicies)
                    ).ConfigureAwait(false);
    }
}