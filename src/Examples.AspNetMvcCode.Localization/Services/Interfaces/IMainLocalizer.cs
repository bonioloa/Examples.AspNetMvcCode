namespace Examples.AspNetMvcCode.Localization;

public interface IMainLocalizer
{
    LocalizedString this[string name] { get; }
    LocalizedString this[string name, params object[] arguments] { get; }
}