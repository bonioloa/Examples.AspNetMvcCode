namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// <see cref="IUrlHelper"/> extension methods.
/// </summary>
public static partial class UrlHelperExtensions
{
    /// <summary>
    /// Generates a fully qualified URL to an action method by using the specified action name, controller name and
    /// route values.
    /// </summary>
    /// <param name="url">The URL helper.</param>
    /// <param name="actionName">The name of the action method.</param>
    /// <param name="controllerName">The name of the controller.</param>
    /// <param name="routeValues">The route values.</param>
    /// <returns>The absolute URL.</returns>
    public static Uri AbsoluteAction(
        this IUrlHelper url
        , string actionName
        , string controllerName
        , object routeValues = null
        )
    {
        return
            new Uri(
                url.Action(
                    actionName
                    , controllerName
                    , routeValues
                    , url.ActionContext.HttpContext.Request.Scheme
                    )
                );
    }



    /// <summary>
    /// Generates a fully qualified URL to the specified content by using the specified content path. Converts a
    /// virtual (relative) path to an application absolute path.
    /// </summary>
    /// <param name="url">The URL helper.</param>
    /// <param name="contentPath">The content path.</param>
    /// <returns>The absolute URL.</returns>
    public static Uri AbsoluteContent(
        this IUrlHelper url
        , string contentPath
        )
    {
        HttpRequest request = url.ActionContext.HttpContext.Request;

        return
            new Uri(
                new Uri(request.Scheme + "://" + request.Host.Value)
                , url.Content(contentPath)
                );
    }



    /// <summary>
    /// Generates a fully qualified URL to the specified route by using the route name and route values.
    /// </summary>
    /// <param name="url">The URL helper.</param>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="routeValues">The route values.</param>
    /// <returns>The absolute URL.</returns>
    public static Uri AbsoluteRouteUrl(
        this IUrlHelper url
        , string routeName
        , object routeValues = null
        )
    {
        return
            new Uri(
                url.RouteUrl(
                    routeName
                    , routeValues
                    , url.ActionContext.HttpContext.Request.Scheme)
                );
    }



    public static string ActionFromRouteValueDictionary(
        this IUrlHelper url
        , RouteValueDictionary route
        )
    {
        string action = route[RouteParams.Action].ToString();
        string controller = route[RouteParams.Controller].ToString();

        route.Remove(RouteParams.Action);
        route.Remove(RouteParams.Controller);

        return
            url.Action(action, controller, route);
    }
}