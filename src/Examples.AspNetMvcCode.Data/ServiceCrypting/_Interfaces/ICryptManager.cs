namespace Examples.AspNetMvcCode.Data;

public interface ICryptManager
{
    byte[] DecryptFile(byte[] toDeCrypt);
    string DecryptAndCleanString(string toDeCrypt);
    byte[] EncryptFile(byte[] toCrypt);
    string CleanAndEncryptString(string toCrypt);
}