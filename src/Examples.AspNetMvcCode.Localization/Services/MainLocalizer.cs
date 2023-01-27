namespace Examples.AspNetMvcCode.Localization;

/// <summary>
/// use this class ONLY to get simple strings. 
/// DO NOT wrap outputs of this class with Html.Raw or HtmlString 
/// USE HtmlLocalizer
/// </summary>
public class MainLocalizer : StringLocalizer<LocalizedStr>, IMainLocalizer
{
    private readonly IStringLocalizer _internalLocalizer;

    public MainLocalizer(
        IStringLocalizerFactory factory
        ) : base(factory)
    {
        _internalLocalizer = new StringLocalizer<LocalizedStr>(factory);
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