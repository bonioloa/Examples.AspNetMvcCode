namespace Examples.AspNetMvcCode.Web.Code;

//https://github.com/VahidN/DNTCaptcha.Core
public class CaptchaWeb : ICaptchaWeb
{
    private readonly IHttpContextAccessorWeb _webHttpContextAccessor;
    private readonly ILogger<CaptchaWeb> _logger;
    private readonly IMainLocalizer _localizer;
    private readonly IDNTCaptchaValidatorService _validatorService;
    private readonly IOptions<DNTCaptchaOptions> _optDntCaptcha;//these are injected at startup by DNTCaptcha package

    public CaptchaWeb(
        IHttpContextAccessorWeb webHttpContextAccessor
        , ILogger<CaptchaWeb> logger
        , IMainLocalizer localizer
        , IDNTCaptchaValidatorService validatorService
        , IOptions<DNTCaptchaOptions> optDntCaptcha
        )
    {
        _webHttpContextAccessor = webHttpContextAccessor;
        _logger = logger;
        _localizer = localizer;
        _validatorService = validatorService;
        _optDntCaptcha = optDntCaptcha;
    }



    public bool ValidateCaptchaRequestIfNeeded()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                    new Dictionary<string, object>
                    {
                        { AppLogPropertiesKeys.MethodName, nameof(ValidateCaptchaRequestIfNeeded) }
                    });

        _logger.LogDebug("CALL");



        //captcha generator language does not matter for SumOfTwoNumbers generation
        if (!_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.SumOfTwoNumbers))
        {
            _webHttpContextAccessor.SessionOperationResult =
               new OperationResultViewModel()
               {
                   LocalizedMessage = _localizer[nameof(LocalizedStr.CaptchaErrorMessage)],
               };

            return false;
        }

        return true;
    }
}