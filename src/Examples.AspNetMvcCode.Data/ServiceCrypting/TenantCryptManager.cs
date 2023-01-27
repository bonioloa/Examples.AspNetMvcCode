namespace Examples.AspNetMvcCode.Data;

public class TenantCryptManager : CryptManager, ITenantCryptManager
{
    private void Initialize(
        CryptingMethods cryptingMethod
        , string cryptKeyPart
        )
    {
        CryptingMethod = cryptingMethod;
        CryptKeyPart = cryptKeyPart;
    }



    public void InitializeWithStandardMethod(
        string cryptKeyPart
        )
    {
        Initialize(
            DefaultCryptingMethod
            , cryptKeyPart
            );
    }
}