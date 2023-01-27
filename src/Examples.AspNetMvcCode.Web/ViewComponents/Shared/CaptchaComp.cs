namespace Examples.AspNetMvcCode.Web.ViewComponents;

public class CaptchaComp : ViewComponent
{
    private readonly ILogger<CaptchaComp> _logger;

    public CaptchaComp(
        ILogger<CaptchaComp> logger
        )
    {
        _logger = logger;
    }



    public async Task<IViewComponentResult> InvokeAsync()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(InvokeAsync) }
                });

        //_logger.LogDebug("CALL");



        return
            await Task.FromResult<IViewComponentResult>(View(MvcComponents.SharedViewCompCaptcha))
                      .ConfigureAwait(false);
    }
}