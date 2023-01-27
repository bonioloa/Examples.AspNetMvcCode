namespace Examples.AspNetMvcCode.Data;

public class DefaultCryptManager : CryptManager, IDefaultCryptManager
{
    public DefaultCryptManager()
    {
        CryptingMethod = DefaultCryptingMethod;
        CryptKeyPart = DefaultCryptingSubkey;
    }
}