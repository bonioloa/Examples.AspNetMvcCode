namespace Examples.AspNetMvcCode.Data;

public interface IDataCommandManagerTenant : IDataCommandManager
{
    void ValidateAndInitialize(TenantConfigQr tenantConfig);
}