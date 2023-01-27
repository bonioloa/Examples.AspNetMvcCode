namespace Examples.AspNetMvcCode.Web;

/// <summary>
/// Add here generic extension methods for <see cref="IApplicationBuilder"/> object 
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// rules for translation from old website direct links to new website corresponding links
    /// </summary>
    /// <param name="app"></param>
    public static void AddRewriteRules(this IApplicationBuilder app)
    {
        //how to use: 
        //add original url regex, WITHOUT querystring
        //add as replacement an action to redirect to our application correct link, WITHOUT querystring
        //no rename or handling needed for querystring values, just use the same parameter name in rewritten action
        //do not try to handle querystring in rewriting, this interferes with the language routing defined in application

        //more information
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/url-rewriting?view=aspnetcore-3.1
        //https://www.hanselman.com/blog/UpgradingA10YearOldSiteToASPNETCoresRazorPagesUsingTheURLRewritingMiddleware.aspx

        #region rewrite for old site links and use a redirect url

        RewriteOptions rewriteOptions =
            new RewriteOptions()


            .AddRewrite(
                @"^oldloginpage.asp"
                , $"/{SupportedCulturesConstants.IsoCodeDefault}/{MvcComponents.CtrlFallback}/{MvcComponents.ActRedirectToLoginTenant}"
                , skipRemainingRules: true
                )
            ;


        app.UseRewriter(rewriteOptions);

        #endregion
    }
}