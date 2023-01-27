namespace Comunica.ProcessManager.Web.ViewComponents;

public class LanguageSelectorViewComponent : ViewComponent
{
    private readonly ILogger<LanguageSelectorViewComponent> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;
    private readonly ICultureMapperWeb _webCultureMapper;


    public LanguageSelectorViewComponent(
        ILogger<LanguageSelectorViewComponent> logger
        , IOptionsSnapshot<ProductSettings> optProduct
        , ICultureMapperWeb webCultureMapper
        )
    {
        _logger = logger;
        _optProduct = optProduct;
        _webCultureMapper = webCultureMapper;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        _logger.LogAppDebug("CALL");

        if (_optProduct.Value.DisableLanguageSelector)
        {
            return await Task.FromResult<IViewComponentResult>(
              Content(string.Empty)
              ).ConfigureAwait(false);
        }


        //validation on route language at this level are already performed
        //so we can safely retrieve display language model from this constant
        IList<CultureViewModel> model = _webCultureMapper.GetEnabledByContextOrAppConfig();

        if (model != null
            && model.HasValues()
            && model.Count > 1)//show select control only if more than 1 language is configured
        {
            return await Task.FromResult<IViewComponentResult>(
              View(MvcComponents.SharedViewCompLanguageSelector, model)
              ).ConfigureAwait(false);
        }
        //don't show selector
        return await Task.FromResult<IViewComponentResult>(
              Content(string.Empty)
              ).ConfigureAwait(false);
    }
}
