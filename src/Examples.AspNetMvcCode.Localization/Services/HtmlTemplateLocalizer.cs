namespace Examples.AspNetMvcCode.Localization;

public class HtmlTemplateLocalizer : HtmlLocalizer<HtmlTemplateLocalized>, IHtmlTemplateLocalizer
{
    private readonly IHtmlLocalizer _internalLocalizer;
    public HtmlTemplateLocalizer(
        IHtmlLocalizerFactory factory
        ) : base(factory)
    {
        _internalLocalizer = new HtmlLocalizer<HtmlTemplateLocalized>(factory);
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