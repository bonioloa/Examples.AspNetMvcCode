namespace Examples.AspNetMvcCode.Localization;

public interface ITemplateLocalizer
{
    LocalizedString this[string name] { get; }
    LocalizedString this[string name, params object[] arguments] { get; }
}