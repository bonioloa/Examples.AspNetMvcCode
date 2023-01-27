namespace Comunica.ProcessManager.Web.Code;

public class RecaptchaWeb : IRecaptchaWeb
{
    private readonly IHttpContextAccessorCustom _httpContextAccessorCustomWeb;
    private readonly ILogger<RecaptchaWeb> _logger;
    private readonly MainLocalizer _localizer;
    private readonly IRecaptchaService _recaptcha;

    public RecaptchaWeb(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , ILogger<RecaptchaWeb> logger
        , MainLocalizer localizer
        , IRecaptchaService recaptcha
        )
    {
        _httpContextAccessorCustomWeb = httpContextAccessorCustomWeb;
        _logger = logger;
        _localizer = localizer;
        _recaptcha = recaptcha;
    }




    public bool ValidateReCaptchaRequestIfNeeded()
    {
        _logger.LogAppDebug($"CALL");

        //if (_httpContextAccessorCustomWeb.SessionReCaptchaAlreadySolvedOnce)
        //{
        //    return true;
        //}

        Task<RecaptchaResponse> recaptcha =
            Task.Run(async () =>
                        await _recaptcha.Validate(_httpContextAccessorCustomWeb.HttpContext.Request)
                                        .ConfigureAwait(false)
                    );
        recaptcha.Wait();
        if (recaptcha.Result.success)
        {
            //_httpContextAccessorCustomWeb.SessionReCaptchaAlreadySolvedOnce = true;
            return true;
        }
        else
        {
            _httpContextAccessorCustomWeb.SessionOperationResult =
               new OperationResultViewModel()
               {
                   LocalizedMessage = _localizer[nameof(LocalizedStr.SharedErrorReCaptcha)],
               };
            return false;
        }
    }
}
