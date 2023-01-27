namespace Comunica.ProcessManager.Web.Code;

public static class ActionExecutingContextExtensions
{
    public static string GetLanguage(this ActionExecutingContext context)
    {
        return context?.HttpContext?.Features?.Get<IRequestCultureFeature>()
            .RequestCulture.Culture.TwoLetterISOLanguageName;
    }
    public static string GetAction(this ActionExecutingContext context)
    {
        return (string)context?.RouteData.Values[RouteParams.Action];
    }
    public static string GetController(this ActionExecutingContext context)
    {
        return (string)context?.RouteData.Values[RouteParams.Controller];
    }
}
