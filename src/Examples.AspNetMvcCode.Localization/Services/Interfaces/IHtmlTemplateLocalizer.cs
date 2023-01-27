namespace Examples.AspNetMvcCode.Localization;

public interface IHtmlTemplateLocalizer
{
    LocalizedHtmlString this[string name] { get; }
    LocalizedHtmlString this[string name, params object[] arguments] { get; }
}