namespace Examples.AspNetMvcCode.Data;

public interface ITenantCryptManager : ICryptManager
{
    void InitializeWithStandardMethod(
        string cryptKeyPart
        );
}