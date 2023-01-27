namespace Examples.AspNetMvcCode.Data;

internal static class ContextAppExtensions
{
    internal static string LanguageSuffix(this ContextApp context)
    {
        return DbUtility.GetOldSchemaLangSuffix(context.CurrentCultureIsoCode);
    }
}