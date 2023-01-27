namespace Examples.AspNetMvcCode.Localization;

/// <summary>
/// use this class to get Html string instead of using Html.Raw or HtmlString 
/// on a StringLocalizer (MainLocalizer)
/// 
/// Warning: .AddViewLocalization() must be used in startup or 
/// asp core will not inject IHtmlLocalizerFactory
/// </summary>
public class HtmlMainLocalizer : HtmlLocalizer<HtmlLocalization>, IHtmlMainLocalizer
{
    private readonly IHtmlLocalizer _internalLocalizer;
    public HtmlMainLocalizer(
        IHtmlLocalizerFactory factory
        ) : base(factory)
    {
        _internalLocalizer = new HtmlLocalizer<HtmlLocalization>(factory);
    }


    public override LocalizedHtmlString this[string name, params object[] arguments]
    {
        get
        {
            return _internalLocalizer[name, arguments];
        }
    }


    public override LocalizedHtmlString this[string name]
    {
        get
        {
            return _internalLocalizer[name];
        }
    }
}