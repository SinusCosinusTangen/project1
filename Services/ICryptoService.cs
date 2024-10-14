namespace project1.Services
{
    public interface ICryptoService
    {
        string GetPublicKey();
        string GenerateKey(string publicKeyPath, string privateKeyPath);
        string HashSha256(string text);
        string EncryptMessage(string publicKey, string message);
        string DecryptMessage(string encryptedMessage);
    }
}
