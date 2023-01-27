using Microsoft.Net.Http.Headers;

namespace Examples.AspNetMvcCode.Web.Controllers;

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

    private readonly IEmailSendBaseLogic _logicEmailSendBase;

    private readonly IMainLocalizer _localizer;
    private readonly IHtmlMainLocalizer _htmlLocalizer;

    public ErroriController(
        IHttpContextAccessorWeb webHttpContextAccessor
        , IOptionsSnapshot<EmailUfSupportSettings> optEmailUfSupport
        , ILogger<ErroriController> logger
        , ContextApp contextApp
        , ContextTenant contextTenant
        , ContextUser contextUser
        , IEmailSendBaseLogic logicEmailSendBase
        , IMainLocalizer localizer
        , IHtmlMainLocalizer htmlLocalizer
        ) : base(webHttpContextAccessor)
    {
        _logger = logger;
        _optEmailUfSupport = optEmailUfSupport;
        _contextApp = contextApp;
        _contextUser = contextUser;
        _contextTenant = contextTenant;
        _logicEmailSendBase = logicEmailSendBase;
        _localizer = localizer;
        _htmlLocalizer = htmlLocalizer;
    }




    public IActionResult Avviso()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(Avviso) }
                });



        IExceptionHandlerPathFeature exceptionFeature =
            _webHttpContextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

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

        string exceptionLog = sb.ToString();
        _logger.LogError("Avviso Exception log: {ExceptionLog}", exceptionLog);

        //finally send email with basic information
        _logicEmailSendBase.SendSupportEmail(
            SupportMailType.GenericError
            , "Error"
            , emailContent
            );


        return
            View(
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
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(Problema) }
                });



        bool shouldBeIgnored = false;

        //this is to prevent log/email spamming due to bots trying to crawl in site
        if (UserAgentIsKnownCrawlerBot())
        {
            shouldBeIgnored = true;
        }


        StringBuilder debugMessage = GetStandardDebugString();
        if (!shouldBeIgnored)
        {
            _logger.LogError(
                "found error html code '{Codice}'. Debug :{DebugMessage}"
                , codice
                , debugMessage
                );
        }

        //non localizziamo tutti i messaggi di
        ErrorViewModel errorModel =
            codice switch
            {
                404 =>
                    new ErrorViewModel()
                    {
                        Title = _localizer[nameof(LocalizedStr.ErrorPageNotFoundTitle)],
                        SubTitle = null,
                        Message = _htmlLocalizer[nameof(HtmlLocalization.ErrorPageNotFoundMessage)],
                    },

                _ =>
                    new ErrorViewModel()
                    {
                        Title = _localizer[nameof(LocalizedStr.ErrorPageTitle)],
                        SubTitle = _htmlLocalizer[nameof(HtmlLocalization.ErrorPageSubtitleHtml)],
                        Message = _htmlLocalizer[nameof(HtmlLocalization.ErrorPageHtmlMessage)],
                    },
            };


        if (!_optEmailUfSupport.Value.IgnoreEmailForCodes.Contains(codice)
            && !shouldBeIgnored)
        {
            _logicEmailSendBase.SendSupportEmail(
                SupportMailType.CodePage
                , $"Error Code {codice}"
                , $"found error html code '{codice}'. Debug :{debugMessage}"
                );
        }

        //la pagina {} non esiste o alcuni parametri non sono validi
        return View(MvcComponents.ViewErrors, errorModel);
    }



    public IActionResult BrowserNonSupportato()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(BrowserNonSupportato) }
                });



        StringBuilder sb = GetStandardDebugString();
        sb.AppendLine("User has an unsupported browser.");

        string message = sb.ToString();

        if (UserAgentIsKnownCrawlerBot())
        {
            //this is to prevent log/email spamming due to bots trying to crawl in site
            _logger.LogInformation("Probably a crawler bot: {Message}", message);

            return View(MvcComponents.ViewNotSupportedBrowser);
        }


        _logger.LogError("Browser unsupported error: {Message}", message);

        _logicEmailSendBase.SendSupportEmail(
            SupportMailType.WrongBrowser
            , "Browser Not Supported Error"
            , message
            );

        return View(MvcComponents.ViewNotSupportedBrowser);
    }


    public IActionResult Manutenzione()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(Manutenzione) }
                });



        IExceptionHandlerPathFeature exceptionFeature =
            _webHttpContextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        StringBuilder debugStringSb = GetStandardDebugString();

        string emailBody;
        string emailSubject;
        if (exceptionFeature != null)
        {
            debugStringSb.AppendLine($"Route eccezione: '{exceptionFeature.Path}';");

            emailBody = debugStringSb.ToString();
            emailBody += "Necessaria verifica della request nei log";

            string exceptionMessage = $"Messaggio eccezione: '{exceptionFeature.Error.Message}';";//do not log exception type, not needed
            //add further information for log

            debugStringSb.AppendLine(exceptionMessage);

            string exceptionLog = debugStringSb.ToString();
            _logger.LogError("Avviso Exception log: {ExceptionLog}", exceptionLog);

            emailSubject = "Error";
        }
        else
        {
            emailSubject = "Maintenance";
            emailBody = debugStringSb.ToString();
            emailBody += $"Nessuna eccezione rilevata. Potrebbe essere un semplice accesso utente se è stata configurata la pagina per bloccare l'accesso alla configurazione da parte degli utenti";
        }

        //finally send email with basic information
        _logicEmailSendBase.SendSupportEmail(
            SupportMailType.GenericError
            , emailSubject
            , emailBody
            );

        return View(MvcComponents.ViewMaintenance);
    }





    #region private helping methods

    [NonAction]
    private string GetRequestId()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(GetRequestId) }
                });



        string requestId =
            Activity.Current?.Id ?? _webHttpContextAccessor?.HttpContext?.TraceIdentifier;

        if (requestId.Empty())
        {
            requestId = string.Empty;
            _logger.LogError("request Id is empty");
        }

        return requestId;
    }


    [NonAction]
    private string GetUserAgentSafe()
    {
        string userAgent =
            Request?.Headers[HeaderNames.UserAgent];

        if (userAgent.Empty())
        {
            userAgent = string.Empty;//prevent null exception in eventual interpolation writings
        }

        return userAgent;
    }


    private bool UserAgentIsKnownCrawlerBot()
    {
        string userAgent = GetUserAgentSafe();

        bool isKnownCrawlerBot =
            _optEmailUfSupport.Value.IgnoreEmailErrorCrawlerBots.Any(bstr => userAgent.ContainsInvariant(bstr));

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