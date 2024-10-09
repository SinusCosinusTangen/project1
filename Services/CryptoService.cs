using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace project1.Services
{
	public class CryptoService()
    {
        public string GetPublicKey()
        {
            string publicKeyPath = GetKeyPath("public");
            string privateKeyPath = GetKeyPath("private");

            if (File.Exists(publicKeyPath) && File.Exists(privateKeyPath))
            {
                using (StreamReader publicKey = new StreamReader(publicKeyPath, true))
                {
                    return publicKey.ReadToEnd();
                }
            }
            else
            {
                return GenerateKey(publicKeyPath, privateKeyPath);
            }
        }

        public string GenerateKey(string publicKeyPath, string privateKeyPath)
        {
            if (File.Exists(publicKeyPath) && File.Exists(privateKeyPath))
            {
                Console.WriteLine("Key files already exists");
            }
            else
            {
                if (File.Exists(publicKeyPath))
                {
                    File.Delete(publicKeyPath);
                }
                else if (File.Exists(privateKeyPath))
                {
                    File.Delete(publicKeyPath);
                }

                using (RSA rsa = RSA.Create(2048))
                {
                    string publicKey = ExportPublicKey(rsa);
                    string privateKey = ExportPrivateKey(rsa);
                    
                    using (StreamWriter outputFile = new StreamWriter(publicKeyPath, true))
                    {
                        outputFile.WriteLine(publicKey);
                    }

                    using (StreamWriter outputFile = new StreamWriter(privateKeyPath, true))
                    {
                        outputFile.WriteLine(privateKey);
                    }
                }
            }

            using (StreamReader publicKey = new StreamReader(publicKeyPath, true))
            {
                return publicKey.ReadToEnd();
            }
        }

        private static string GetKeyPath(string keyType)
        {
            string? assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? "";
            DirectoryInfo? assemblyDir = Directory.GetParent(assemblyLocation);
            if (assemblyDir == null || assemblyDir.Parent == null || assemblyDir.Parent.Parent == null)
            {
                throw new InvalidOperationException("Unable to determine the project root directory");
            }
            
            string projectDir = assemblyDir.Parent.Parent.FullName;

            if (keyType == "private")
            {
                return Path.Combine(projectDir, "private.pem");
            }
            else
            {
                return Path.Combine(projectDir, "public.pem");
            }
        }

        public static string HashSha256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256 hashstring = SHA256.Create();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }

        public static string ExportPrivateKey(RSA rsa)
        {
            var privateKeyBytes = rsa.ExportPkcs8PrivateKey();
            return "-----BEGIN PRIVATE KEY-----\n" +
                Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                "\n-----END PRIVATE KEY-----";
        }

        public static string ExportPublicKey(RSA rsa)
        {
            var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
            return "-----BEGIN PUBLIC KEY-----\n" +
                Convert.ToBase64String(publicKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                "\n-----END PUBLIC KEY-----";
        }

        public static string EncryptMessage(string publicKey, string message)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(publicKey.ToCharArray());
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] encryptedBytes = rsa.Encrypt(messageBytes, RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public static string DecryptMessage(string encryptedMessage)
        {
            string privateKeyPath = GetKeyPath("private");
            StreamReader privateKeyReader = new(privateKeyPath, true);
            string privateKey = privateKeyReader.ReadToEnd() ?? "";
            
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKey.ToCharArray());
                byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
                byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
                privateKeyReader.Close();

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}