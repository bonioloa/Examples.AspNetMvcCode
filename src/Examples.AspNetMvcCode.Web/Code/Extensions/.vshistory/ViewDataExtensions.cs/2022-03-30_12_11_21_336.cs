namespace Comunica.ProcessManager.Web.Code;

//why this clusterfuck? to handle null values without repeating code
//and because framework  randomly loses viewbag values during debug
public static class ViewDataExtensions
{
    public static void SetTitle(this ViewDataDictionary tempData, string title)
    {
        tempData["Title"] = title;
    }
    public static string GetTitle(this ViewDataDictionary tempData)
    {
        return tempData.GetString("Title");
    }


    public static void SetBodyClasses(this ViewDataDictionary tempData, string bodyClasses)
    {
        tempData["BodyClasses"] = bodyClasses;
    }
    public static string GetBodyClasses(this ViewDataDictionary tempData)
    {
        return tempData.GetString("BodyClasses");
    }


    public static void SetContainerClasses(this ViewDataDictionary tempData, string containerClasses)
    {
        tempData["ContainerClasses"] = containerClasses;
    }
    public static string GetContainerClasses(this ViewDataDictionary tempData)
    {
        return tempData.GetString("ContainerClasses");
    }


    public static void SetWrapOfficialBackground(this ViewDataDictionary tempData, bool wrapOfficialBackground)
    {
        tempData["WrapOfficialBackground"] = wrapOfficialBackground;
    }
    public static bool GetWrapOfficialBackground(this ViewDataDictionary tempData)
    {
        return tempData.GetBool("WrapOfficialBackground");
    }


    public static void SetUseRecaptcha(this ViewDataDictionary tempData, bool useRecaptcha)
    {
        tempData["UseRecaptcha"] = useRecaptcha;
    }
    public static bool GetUseRecaptcha(this ViewDataDictionary tempData)
    {
        return tempData.GetBool("UseRecaptcha");
    }


    public static void SetUseDatatableNetJsLibrary(this ViewDataDictionary tempData, bool useDatatableNetJsLibrary)
    {
        tempData["UseDatatableNetJsLibrary"] = useDatatableNetJsLibrary;
    }
    public static bool GetUseDatatableNetJsLibrary(this ViewDataDictionary tempData)
    {
        return tempData.GetBool("UseDatatableNetJsLibrary");
    }


    public static void SetUseChartJsLibrary(this ViewDataDictionary tempData, bool useChartJsLibrary)
    {
        tempData["UseChartJsLibrary"] = useChartJsLibrary;
    }
    public static bool GetUseChartJsLibrary(this ViewDataDictionary tempData)
    {
        return tempData.GetBool("UseChartJsLibrary");
    }


    public static void SetUseDynamicFormComponent(this ViewDataDictionary tempData, bool useDynamicFormComponent)
    {
        tempData["UseDynamicFormComponent"] = useDynamicFormComponent;
    }
    public static bool GetUseDynamicFormComponent(this ViewDataDictionary tempData)
    {
        return tempData.GetBool("UseDynamicFormComponent");
    }


    public static void SetShowLanguageSelector(this ViewDataDictionary tempData, bool showLanguageSelector)
    {
        tempData["ShowLanguageSelector"] = showLanguageSelector;
    }
    public static bool GetShowLanguageSelector(this ViewDataDictionary tempData)
    {
        return tempData.GetBool("ShowLanguageSelector");
    }


    public static void SetUseGridExportToFile(this ViewDataDictionary tempData, bool useGridExportToFile)
    {
        tempData["UseGridExportToFile"] = useGridExportToFile;
    }
    public static bool GetUseGridExportToFile(this ViewDataDictionary tempData)
    {
        return tempData.GetBool("UseGridExportToFile"); 
    }


    public static void SetLogo(this ViewDataDictionary tempData, InfoAndLogo logo)
    {
        tempData["Logo"] = logo;
    }
    public static InfoAndLogo GetLogo(this ViewDataDictionary tempData)
    {
        return tempData.GetInfoAndLogo("Logo");
    }


    public static void SetLeftPanel(this ViewDataDictionary tempData, InfoAndLogo panelType)
    {
        tempData["LeftPanel"] = panelType;
    }
    public static InfoAndLogo GetLeftPanel(this ViewDataDictionary tempData)
    {
        return tempData.GetInfoAndLogo("LeftPanel");
    }


    public static void SetRightPanel(this ViewDataDictionary tempData, InfoAndLogo panelType)
    {
        tempData["RightPanel"] = panelType;
    }
    public static InfoAndLogo GetRightPanel(this ViewDataDictionary tempData)
    {
        return tempData.GetInfoAndLogo("RightPanel");
    }




    private static string GetString(this ViewDataDictionary tempData, string key)
    {
        return tempData != null && tempData[key] != null
            ? tempData[key].ToString()
            : string.Empty;
    }
    private static bool GetBool(this ViewDataDictionary tempData, string key)
    {
        return tempData != null && tempData[key] != null
                && (bool)tempData[key];
    }
    private static InfoAndLogo GetInfoAndLogo(this ViewDataDictionary tempData, string key)
    {
        return tempData != null && tempData[key] != null
           ? (InfoAndLogo)tempData[key]
           : InfoAndLogo.Missing;
    }
}