namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class ProductTenantInfoAndLogoComp : ViewComponent
{
    private readonly ILogger<ProductTenantInfoAndLogoComp> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;
    private readonly ContextTenant _contextTenant;

    private readonly IPersonalizationLogic _logicPersonalization;

    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;

    public ProductTenantInfoAndLogoComp(
        ILogger<ProductTenantInfoAndLogoComp> logger
        , IOptionsSnapshot<ProductSettings> optProduct
        , ContextTenant contextTenant
        , IPersonalizationLogic logicPersonalization
        , IHttpContextAccessorWeb webHttpContextAccessor
        )
    {
        _logger = logger;
        _optProduct = optProduct;
        _contextTenant = contextTenant;
        _logicPersonalization = logicPersonalization;
        _webHttpContextAccessor = webHttpContextAccessor;
    }



    public async Task<IViewComponentResult> InvokeAsync(InfoAndLogoInputViewModel model)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(InvokeAsync) }
                });

        _logger.LogDebug("CALL");



        Guard.Against.Null(model, nameof(InfoAndLogoInputViewModel));


        IViewComponentResult viewResult = Content(string.Empty);
        bool error = false;
        //SessionHideLoginLeftPanel means 
        //we have to hide left panel and show logo on right panel
        //we have then to detect if we have to show product info/logo
        //or tenant info/logo

        //Panel flags don't depend on SessionHideLoginLeftPanel
        //these flags are used in views where component must be directly shown
        bool isTenantLoginWithoutLeftPanel =
            _webHttpContextAccessor.IsTenantLoginWithoutLeftPanel();


        switch (model.InfoAndLogoType)
        {
            case InfoAndLogo.CentralPanelNoLogo:
                viewResult = Content(string.Empty);
                break;


            case InfoAndLogo.CentralPanelLogoProduct:
                viewResult =
                    View(
                        MvcComponents.SharedViewCompCentralPanelLogo
                        , BuildModel(useProduct: true)
                        );
                break;


            case InfoAndLogo.CentralPanelLogoTenant:
                viewResult =
                    View(
                        MvcComponents.SharedViewCompCentralPanelLogo
                        , BuildModel(useProduct: false)
                        );
                break;


            case InfoAndLogo.LeftPanelProduct:
                if (!isTenantLoginWithoutLeftPanel)
                {
                    viewResult =
                        View(
                            MvcComponents.SharedViewCompLeftPanel
                            , BuildModel(useProduct: true)
                            );
                }
                break;


            case InfoAndLogo.LeftPanelTenant:
                if (!isTenantLoginWithoutLeftPanel)
                {
                    viewResult =
                        View(
                            MvcComponents.SharedViewCompLeftPanel
                            , BuildModel(useProduct: false)
                            );
                }
                break;


            case InfoAndLogo.RightPanelLogoProduct:
                if (isTenantLoginWithoutLeftPanel)
                {
                    viewResult =
                        View(
                            MvcComponents.SharedViewCompRightPanelLogo
                            , BuildModel(useProduct: true)
                            );
                }
                break;


            case InfoAndLogo.RightPanelLogoTenant:
                if (isTenantLoginWithoutLeftPanel)
                {
                    viewResult =
                        View(
                            MvcComponents.SharedViewCompRightPanelLogo
                            , BuildModel(useProduct: false)
                            );
                }
                break;


            default:
                error = true;
                break;
        }
        if (error)
        {
            throw new PmWebException($"unrecognized {nameof(InfoAndLogo)} value '{model.InfoAndLogoType}' ");
        }


        return
            await Task.FromResult(viewResult)
                      .ConfigureAwait(false);
    }



    private InfoAndLogoViewModel BuildModel(bool useProduct)
    {
        if (useProduct)
        {
            return
                new InfoAndLogoViewModel(
                    LogoPath: base.Url.Content(PathsStaticFilesRoot.AppPathProductsLogoPath + _optProduct.Value.ProductLogoFileName)
                    , LogoAltText: _optProduct.Value.ProductLogoImageAltText
                    , AdditionalLogoPath: string.Empty//leave always empty
                    , AdditionalLogoPathText: string.Empty
                    , WebsiteLink: _optProduct.Value.ProductWebsite
                    , WebsiteDisplayText: _optProduct.Value.ProductWebsiteText
                    , Email: _optProduct.Value.ProductSupportEmail
                    , EmailDisplayText: _optProduct.Value.ProductSupportEmailText
                    );
        }


        if (_contextTenant is null
            || _contextTenant.LogoFileName.Empty())
        {
            throw new PmWebException($"{nameof(ContextTenant)} is not initialized");
        }

        CompanyGroupInfoLgc companyGroupInfo = _logicPersonalization.GetInfo();
        TenantAreaDisplayLgc tenantAreaDisplay = _logicPersonalization.GetPreferences();


        return
             new InfoAndLogoViewModel(
                    LogoPath: base.Url.Content(PathsStaticFilesAdditional.AppPathTenantsLogo + _contextTenant.LogoFileName)
                    , LogoAltText: string.Empty //TODO testo alternativo ad immagine per tenant

                    , AdditionalLogoPath:
                        tenantAreaDisplay.ShowProductLogoInUserLoginPage
                        ? base.Url.Content(PathsStaticFilesRoot.AppPathProductsLogoPath + _optProduct.Value.ProductLogoFileName)
                        : string.Empty

                    , AdditionalLogoPathText:
                        tenantAreaDisplay.ShowProductLogoInUserLoginPage
                        ? _optProduct.Value.ProductLogoImageAltText
                        : string.Empty

                    , WebsiteLink: companyGroupInfo.WebsiteLink
                    , WebsiteDisplayText: companyGroupInfo.WebsiteDisplayText
                    , Email: companyGroupInfo.Email
                    , EmailDisplayText: companyGroupInfo.EmailDisplayText
                    );
    }
}