namespace Examples.AspNetMvcCode.Web.Code;

public interface ICultureMapperWeb
{
    IList<string> GetAppSupportedCulturesList();
    IList<CultureViewModel> GetEnabledByContextOrAppConfig();
    bool SetCultureAndDetectIfRedirectNeeded();
}