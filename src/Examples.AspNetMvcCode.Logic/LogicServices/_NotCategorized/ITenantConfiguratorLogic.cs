namespace Examples.AspNetMvcCode.Logic;

public interface ITenantConfiguratorLogic
{
    SsoConfigLgc GetSsoConfigByIdOrDefault(long ssoConfigId);
    TenantProfileLgc ValidateAndSetTenantContext(string tenantToken, IPAddress remoteIpAddress);
    OperationResultLgc ValidateProfileAndSetTenantContext(TenantProfileLgc claimTenantProfileToValidate, IPAddress remoteIpAddress);
}