namespace Examples.AspNetMvcCode.Localization;

public static class LocalizationConstants
{
    //path with application should always be <iis appsubpath>/<culture>/<remaining url> .
    //https://<domain> is excluded from path
    public const int UrlPathCultureIndex = 1;
}