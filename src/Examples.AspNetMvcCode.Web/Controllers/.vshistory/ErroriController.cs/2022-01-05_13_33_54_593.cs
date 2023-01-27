using Microsoft.Net.Http.Headers;

namespace Comunica.ProcessManager.Web.Controllers;

/// <summary>
/// Don't decorate the error handler action method with HTTP method attributes, such as HttpGet. 
/// Explicit verbs prevent some requests from reaching the method. 
/// Allow anonymous access to the method so that unauthenticated users are able to receive the error view.
/// https://docs.microsoft.com/it-it/aspnet/core/fundamentals/error-handling?view=aspnetcore-3.0
/// </summary>
[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErroriController : BaseContextController
{
    private readonly ILogger<ErroriController> _logger;
    private readonly IOptionsSnapshot<EmailUfSupportSettings> _optEmailUfSupport;

    private readonly ContextApp _contextApp;
    private readonly ContextTenant _contextTenant;
    private readonly ContextUser _contextUser;

    private readonly MainLocalizer _localizer;
    private readonly HtmlMainLocalizer _htmlLocalizer;
    private readonly IEmailWeb _webEmail;

    public ErroriController(
        IHttpContextAccessorCustom httpContextAccessorCustomWeb
        , IOptionsSnapshot<EmailUfSupportSettings> optEmailUfSupport
        , ILogger<ErroriController> logger
        , ContextApp contextApp
        , ContextTenant contextTenant
        , ContextUser contextUser
        , MainLocalizer localizer
        , HtmlMainLocalizer htmlLocalizer
        , IEmailWeb webEmail
        ) : base(httpContextAccessorCustomWeb)
    {
        _logger = logger;
        _optEmailUfSupport = optEmailUfSupport;
        _contextApp = contextApp;
        _contextUser = contextUser;
        _contextTenant = contextTenant;
        _localizer = localizer;
        _htmlLocalizer = htmlLocalizer;
        _webEmail = webEmail;
    }




    public IActionResult Avviso()
    {
        IExceptionHandlerPathFeature exceptionFeature =
            _httpContextAccessorCustomWeb.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        StringBuilder sb = GetStandardDebugString();

        string exceptionMessage;
        if (exceptionFeature != null)
        {
            sb.AppendLine($"Route eccezione: '{exceptionFeature.Path}';");
            exceptionMessage = $"Messaggio eccezione: '{exceptionFeature.Error.Message}';";//do not log exception type, not needed       
        }
        else
        {
            exceptionMessage = $"Eccezione vuota;";
        }


        string emailContent = sb.ToString();
        emailContent += "Necessaria verifica della request nei log";


        //add further information for log

        sb.AppendLine(exceptionMessage);

        _logger.LogAppError(sb.ToString());

        //finally send email with basic information
        _webEmail.SendWbSupportNotificationEmail(
            SupportMailType.GenericError
            , "Error"
            , emailContent
            );


        return View(
            MvcComponents.ViewErrors
            , new ErrorViewModel()
            {
                Title = _localizer[nameof(LocalizedStr.ErrorPageTitle)],
                SubTitle = _htmlLocalizer[nameof(HtmlLocalization.ErrorPageSubtitleHtml)],
                Message = _htmlLocalizer[nameof(HtmlLocalization.ErrorPageHtmlMessage)],
            }
        );
    }



    //https://developer.mozilla.org/it/docs/Web/HTTP/Status
    //https://it.wikipedia.org/wiki/Codici_di_stato_HTTP        
    public IActionResult Problema([RequiredFromQuery] int codice)
    {
        bool shouldBeIgnored = false;

        //this is to prevent log/email spamming due to bots trying to crawl in site
        if (UserAgentIsKnownCrawlerBot())
        {
            shouldBeIgnored = true;
        }

        string message = $"found error html code '{codice}'. Debug :{GetStandardDebugString()}";

        if (!shouldBeIgnored)
        {
            _logger.LogAppError(message);
        }

        //non localizziamo tutti i messaggi di
        ErrorViewModel errorModel = codice switch
        {
            404 => new ErrorViewModel()
            {
                Title = _localizer[nameof(LocalizedStr.ErrorPageNotFoundTitle)],
                SubTitle = null,
                Message = _htmlLocalizer[nameof(HtmlLocalization.ErrorPageNotFoundMessage)],
            },
            _ => new ErrorViewModel()
            {
                Title = _localizer[nameof(LocalizedStr.ErrorPageTitle)],
                SubTitle = _htmlLocalizer[nameof(HtmlLocalization.ErrorPageSubtitleHtml)],
                Message = _htmlLocalizer[nameof(HtmlLocalization.ErrorPageHtmlMessage)],
            },
        };

        if (!_optEmailUfSupport.Value.IgnoreEmailForCodes.Contains(codice)
            && !shouldBeIgnored)
        {
            _webEmail.SendWbSupportNotificationEmail(
                SupportMailType.CodePage
                , $"Error Code {codice}"
                , message
                );
        }

        //la pagina {} non esiste o alcuni parametri non sono validi
        return View(MvcComponents.ViewErrors, errorModel);
    }



    public IActionResult BrowserNonSupportato()
    {
        StringBuilder sb = GetStandardDebugString();
        sb.AppendLine("User has an unsupported browser.");

        string message = sb.ToString();

        if (UserAgentIsKnownCrawlerBot())
        {
            //this is to prevent log/email spamming due to bots trying to crawl in site
            _logger.LogAppDebug(message);
        }
        else
        {
            _logger.LogAppError(message);

            _webEmail.SendWbSupportNotificationEmail(
                SupportMailType.WrongBrowser
                , "Browser Not Supported Error"
                , message
                );
        }

        return View(MvcComponents.ViewNotSupportedBrowser);
    }

    public IActionResult Manutenzione()
    {
        return View(MvcComponents.ViewMaintenance);
    }





    #region private helping methods

    [NonAction]
    private string GetRequestId()
    {
        string requestId = Activity.Current?.Id ?? _httpContextAccessorCustomWeb?.HttpContext?.TraceIdentifier;
        if (requestId.Empty())
        {
            requestId = string.Empty;
            _logger.LogAppError("request Id is empty");
        }

        return requestId;
    }

    [NonAction]
    private string GetUserAgentSafe()
    {
        string userAgent = Request?.Headers[HeaderNames.UserAgent];
        if (userAgent.Empty())
        {
            userAgent = string.Empty;//prevent null exception in eventual interpolation writings
        }
        return userAgent;
    }

    private bool UserAgentIsKnownCrawlerBot()
    {
        string userAgent = GetUserAgentSafe();
        bool isKnownCrawlerBot = _optEmailUfSupport.Value.IgnoreEmailErrorCrawlerBots.Any(bstr => userAgent.ContainsInvariant(bstr));
        return isKnownCrawlerBot;
    }

    [NonAction]
    private static string GetEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    [NonAction]
    private StringBuilder GetStandardDebugString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Ambiente: '{GetEnvironment()}'");
        sb.AppendLine($"User Agent: '{GetUserAgentSafe()}';");
        sb.AppendLine($"RequestId: '{GetRequestId()}';");
        sb.AppendLine(GetAllContextSerialization());
        return sb;
    }

    private string GetAllContextSerialization()
    {
        string tmpSerialApp;
        try
        {
            JsonSerializerOptions serializeOptions = new();
            serializeOptions.Converters.Add(new CultureInfoConverter());
            tmpSerialApp = JsonSerializer.Serialize(_contextApp, serializeOptions);
        }
        catch (Exception ex)
        {
            tmpSerialApp = ex.Message;
        }

        string tmpSerialTenant;
        try
        {
            tmpSerialTenant = JsonSerializer.Serialize(_contextTenant);
        }
        catch (Exception ex)
        {
            tmpSerialTenant = ex.Message;
        }

        string tmpSerialUser;
        try
        {
            tmpSerialUser = JsonSerializer.Serialize(_contextUser);
        }
        catch (Exception ex)
        {
            tmpSerialUser = ex.Message;
        }

        return $@"
                All Context data:
                '{nameof(_contextApp)}: 
                    {tmpSerialApp}

                '{nameof(_contextTenant)}':
                    {tmpSerialTenant}

                '{nameof(_contextUser)}': 
                    {tmpSerialUser}'
                ";
    }
    #endregion
}
