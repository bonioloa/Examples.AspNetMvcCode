namespace Examples.AspNetMvcCode.Web.Code;

public static class PathsStaticFilesAdditional
{
    private const string AppFolderBaseStaticFiles = "_yourcustompath";

    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-6.0#serve-files-outside-of-web-root
    public const string AppPathBaseStaticFiles = "/" + AppFolderBaseStaticFiles;//do not use directly this string to compose link files from this folder

    public const string AppPathBaseWebStaticFiles = @"~/" + AppFolderBaseStaticFiles + "/";//base path for link static files

    public const string AppPathStaticDocuments = AppPathBaseWebStaticFiles + "documentfolder/";
    public const string AppPathPrivacyPolicyNoLang = AppPathStaticDocuments + "_site/privacy_";


    public const string AppPathImages = AppPathBaseWebStaticFiles + "imagefolder/";
    public const string AppPathTenantsLogo = AppPathImages + @"tenantlogofolder/";
}