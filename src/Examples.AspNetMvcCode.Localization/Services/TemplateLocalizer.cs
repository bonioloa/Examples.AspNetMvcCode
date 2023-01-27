namespace Examples.AspNetMvcCode.Localization;

public class TemplateLocalizer : StringLocalizer<TemplateLocalized>, ITemplateLocalizer
{
    private readonly IStringLocalizer _internalLocalizer;

    public TemplateLocalizer(
        IStringLocalizerFactory factory
        ) : base(factory)
    {
        _internalLocalizer = new StringLocalizer<TemplateLocalized>(factory);
    }

    public override LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            return _internalLocalizer[name, arguments];
        }
    }

    public override LocalizedString this[string name]
    {
        get
        {
            return _internalLocalizer[name];
        }
    }
}