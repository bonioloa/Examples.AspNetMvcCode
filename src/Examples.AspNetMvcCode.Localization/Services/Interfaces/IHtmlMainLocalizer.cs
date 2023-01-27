namespace Examples.AspNetMvcCode.Localization;

public interface IHtmlMainLocalizer
{
    LocalizedHtmlString this[string name] { get; }
    LocalizedHtmlString this[string name, params object[] arguments] { get; }
}