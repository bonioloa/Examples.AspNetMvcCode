namespace Examples.AspNetMvcCode.Data.Dto;

/// <summary>
/// POCO model for tenant data (connection string and user access mode)
/// </summary>
/// <param name="Token"></param>
/// <param name="ConfigType"></param>
/// <param name="EnableTwoFactorAuthentication"></param>
/// <param name="DisableAccess"></param>
/// <param name="DbName"></param>
/// <param name="DbLogin"></param>
/// <param name="DbPassword"></param>
/// <param name="Description"></param>
public record TenantConfigQr(
    string Token
    , ConfigurationType ConfigType
    , bool EnableTwoFactorAuthentication
    , bool DisableAccess
    , string DbName
    , string DbLogin
    , string DbPassword
    , string Description
    );