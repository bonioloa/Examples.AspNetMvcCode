namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class PoliciesLinksComp : ViewComponent
{
    private readonly ILogger<PoliciesLinksComp> _logger;

    private readonly IPersonalizationLogic _logicPersonalization;

    private readonly IOptionsSnapshot<ProductSettings> _optProduct;

    public PoliciesLinksComp(
        ILogger<PoliciesLinksComp> logger
        , IPersonalizationLogic logicPersonalization
        , IOptionsSnapshot<ProductSettings> optProduct
        )
    {
        _logger = logger;
        _logicPersonalization = logicPersonalization;
        _optProduct = optProduct;
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



        if (_optProduct.Value.DisablePrivacyComponents)
        {
            return
                await Task.FromResult<IViewComponentResult>(
                     Content(string.Empty)
                     ).ConfigureAwait(false);
        }


        return
            await Task.FromResult<IViewComponentResult>(
                        View(
                            MvcComponents.SharedViewCompPoliciesLinks
                            , new PoliciesViewModel(
                                PrivacyPolicies:
                                    _logicPersonalization.GetLinks()
                                                         .MapIListFromLogicToWeb()
                                )
                            )
                        )
                       .ConfigureAwait(false);
    }
}