namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// class for creation of absolute url necessary to application
/// and also needed in multiple controllers, to avoid code duplication
/// </summary>
public static class UrlHelperCustomExtensions
{
    /// <summary>
    /// create absolute url to item management page of provided id
    /// </summary>
    /// <returns></returns>
    public static Uri AbsoluteActionItemManagement(
        this IUrlHelper url
        , long idItem
        )
    {
        return url.AbsoluteAction(
            MvcComponents.ActViewAndManage
            , MvcComponents.CtrlItemManagement
            , new Dictionary<string, string>
                {
                        { ParamsNames.IdItem, idItem.ToString()}
                }
            );
    }

    public static Uri AbsoluteActionAccessPage(
        this IUrlHelper url
        , string token
        )
    {
        if (token.Empty())
        {
            Log.Logger.Error("token must be provided for access page url construction");
            throw new WebAppException();
        }
        return url.AbsoluteAction(
            MvcComponents.ActLoginTenant
            , MvcComponents.CtrlAccessMain
            , new Dictionary<string, string>
                {
                        { ParamsNames.TenantToken, token.ToString()}
                }
            );
    }

    public static string LinkTagWithText(
        this IUrlHelper url
        , string action
        , string controller
        , string displayText
        )
    {
        return $"<a href='{url.Action(action, controller)}'>{displayText}</a>";
    }
}
