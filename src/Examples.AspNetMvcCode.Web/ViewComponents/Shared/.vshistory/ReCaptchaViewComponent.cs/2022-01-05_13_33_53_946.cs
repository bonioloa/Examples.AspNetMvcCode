namespace Comunica.ProcessManager.Web.ViewComponents;

public class ReCaptchaViewComponent : ViewComponent
{
    private readonly ILogger<ReCaptchaViewComponent> _logger;
    private readonly IOptions<RecaptchaSettings> _optRecaptchaSettings;

    public ReCaptchaViewComponent(
        ILogger<ReCaptchaViewComponent> logger
        , IOptions<RecaptchaSettings> optRecaptchaSettings
        )
    {
        _logger = logger;
        _optRecaptchaSettings = optRecaptchaSettings;
    }



    public async Task<IViewComponentResult> InvokeAsync(
        string useProfile
        , string callBackFunction
        )
    {
        _logger.LogAppDebug("CALL");

        ReCaptchaViewModel model = new()
        {
            RecaptchaSettings = _optRecaptchaSettings.Value,
            Profile = useProfile,
            CallbackFunction = callBackFunction,
        };

        return await Task.FromResult<IViewComponentResult>(
             View(MvcComponents.SharedViewCompReCaptcha, model)
             ).ConfigureAwait(false);
    }
}
