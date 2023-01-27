namespace Examples.AspNetMvcCode.Data;

//criptazioni disponibili (riservato)
public enum CryptingMethods
{
    Default,
    //...
}

public class CryptManager : ICryptManager
{
    //only this need to be public, 
    //the rest of the class must be available 
    //only in its own namespace
    public const CryptingMethods DefaultCryptingMethod = CryptingMethods.Default;//riservato
    public static readonly string DefaultCryptingSubkey = string.Empty;

    protected CryptingMethods CryptingMethod { get; set; }
    protected string CryptKeyPart { get; set; }
    protected bool CryptEnabled { get; set; }

    


    #region encrypting

    //implementazione faked perché codice riservato
    private static string CryptString(string toEncrypt, CryptingMethods method, string cryptKeyPart)
    {
        //...
        return toEncrypt;
    }

    /// <summary>
    /// clean and encrypt string using method and key from configuration.
    /// </summary>
    /// <remarks>Don't clean returned encrypted string</remarks>
    /// <param name="toCrypt"></param>
    /// <returns></returns>
    public string CleanAndEncryptString(string toCrypt)
    {
        toCrypt = toCrypt.Clean();

        return
            CryptEnabled
            ? CryptString(
                toCrypt.Clean()
               , CryptingMethod
               , CryptKeyPart
               )
            : toCrypt;
    }

    //implementazione faked perché codice riservato
    private static byte[] CryptFile(byte[] toCrypt, CryptingMethods method, string cryptKeyPart)
    {
        //...
        return toCrypt;
    }

    public byte[] EncryptFile(byte[] toCrypt)
    {
        return CryptFile(
            toCrypt
            , CryptingMethod
            , CryptKeyPart
            );
    }
    #endregion


    #region decrypting

    //implementazione faked perché codice riservato
    private static string DecryptString(string toEncrypt, CryptingMethods method, string cryptKeyPart)
    {
        //...
        return toEncrypt;
    }


    /// <summary>
    /// decrypt and clean string using method and key from configuration.
    /// </summary>
    /// <remarks>Don't clean input string</remarks>
    /// <param name="toDeCrypt"></param>
    /// <returns></returns>
    public string DecryptAndCleanString(string toDeCrypt)
    {
        //ignore CryptEnabled, try decrypting anyway
        return
            DecryptString(
                toDeCrypt
                , CryptingMethod
                , CryptKeyPart
                ).Clean();
    }


    //implementazione faked perché codice riservato
    private static byte[] DecryptFile(byte[] toDeCrypt, CryptingMethods method, string cyptKeyPart)
    {
        //...
        return toDeCrypt;
    }
    public byte[] DecryptFile(byte[] toDeCrypt)
    {
            return
                DecryptFile(
                    toDeCrypt
                    , CryptingMethod
                    , CryptKeyPart
                    );
    }

    #endregion

}