namespace Examples.AspNetMvcCode.Web.Code;

public static class ActionExecutingContextExtensions
{
    public static string GetLanguage(this ActionExecutingContext context)
    {
        return context?.HttpContext?.Features?.Get<IRequestCultureFeature>()
            .RequestCulture.Culture.TwoLetterISOLanguageName;
    }


    public static string GetAction(this ActionExecutingContext context)
    {
        RouteValueDictionary routeValues = context?.RouteData?.Values;
        if(routeValues is not null 
            && routeValues.TryGetValue(RouteParams.Action, out object actionObj)
            && actionObj is string action)
        {
            return action;
        }
        return string.Empty;
    }


    public static string GetController(this ActionExecutingContext context)
    {
        RouteValueDictionary routeValues = context?.RouteData?.Values;
        if (routeValues is not null
            && routeValues.TryGetValue(RouteParams.Controller, out object controllerObj)
            && controllerObj is string controller)
        {
            return controller;
        }
        return string.Empty;
    }
}