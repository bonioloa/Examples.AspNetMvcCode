namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class LanguageSelectorComp : ViewComponent
{
    private readonly ILogger<LanguageSelectorComp> _logger;
    private readonly IOptionsSnapshot<ProductSettings> _optProduct;
    private readonly ICultureMapperWeb _webCultureMapper;


    public LanguageSelectorComp(
        ILogger<LanguageSelectorComp> logger
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
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(InvokeAsync) }
                });

        _logger.LogDebug("CALL");



        if (_optProduct.Value.DisableLanguageSelector)
        {
            return
                await Task.FromResult<IViewComponentResult>(
                      Content(string.Empty)
                      ).ConfigureAwait(false);
        }


        //validation on route language at this level are already performed
        //so we can safely retrieve display language model from this constant
        IList<CultureViewModel> modelOutput = _webCultureMapper.GetEnabledByContextOrAppConfig();


        if (modelOutput != null
            && modelOutput.HasValues()
            && modelOutput.Count > 1)//show select field only if more than 1 language is configured
        {
            return
                await Task.FromResult<IViewComponentResult>(
                                  View(MvcComponents.SharedViewCompLanguageSelector, modelOutput)
                                  )
                           .ConfigureAwait(false);
        }


        //don't show selector
        return
            await Task.FromResult<IViewComponentResult>(
                          Content(string.Empty)
                          )
                       .ConfigureAwait(false);
    }
}