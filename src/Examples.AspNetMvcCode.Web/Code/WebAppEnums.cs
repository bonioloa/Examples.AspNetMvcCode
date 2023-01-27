namespace Examples.AspNetMvcCode.Web.Code;

public enum SharedResultCode
{
    Missing,
    Ok,
    Ko,
}
public enum ItemSaveResultCode
{
    Missing,//default
    KoNoReload,
    KoWithReload,
    //OK,
    OkNoReload,
    OkWithReload,
    OkToItem,
    OkToMainpage,
}
public enum InfoAndLogo
{
    Missing,//default
    CentralPanelNoLogo,
    LeftPanelProduct,
    RightPanelLogoProduct,
    CentralPanelLogoProduct,
    LeftPanelTenant,
    RightPanelLogoTenant,
    CentralPanelLogoTenant,
}

public enum BackUrlConfig
{
    Missing, //invalid value
    TenantLogin,
    PageFromItemManagement, //use this to generate link to return to ItemManagement
    InsertNew,
    AdminApp,
    UserSupervisorSearch,
}

public enum CommandJumpType
{
    Undefined,
    Next,
    Alternative,
}

public enum IncludeType
{
    Css,
    Javascript,
}