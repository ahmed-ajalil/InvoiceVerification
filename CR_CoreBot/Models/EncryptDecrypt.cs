using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CR_CoreBot.Models
{
    public class EncryptDecrypt
    {
        private static Hashtable hs = new Hashtable();
        private const string initVector = "tu89geji340t89u2";
        private const int keysize = 256;
        private static readonly string PasswordToEncrypt = "encrypass";
        private static readonly string InitVector = "Your16ByteVector"; // 16 bytes initialization vector
        private static readonly int KeySize = 256; // Key size in bits

        public static string TextEncrypt(string plainText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var password = new Rfc2898DeriveBytes(PasswordToEncrypt, Encoding.UTF8.GetBytes("salt"), 10000))
            {
                byte[] keyBytes = password.GetBytes(KeySize / 8);

                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                string urlSafeEncryptedText = Uri.EscapeDataString(Convert.ToBase64String(cipherTextBytes));
                                return urlSafeEncryptedText;
                            }
                        }
                    }
                }
            }
        }

        public static string TextDecrypt(string cipherText)
        {


            string readerFinal = string.Empty;
            try
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                // Generate a key using Rfc2898DeriveBytes
                using (var password = new Rfc2898DeriveBytes(PasswordToEncrypt, Encoding.UTF8.GetBytes("salt"), 10000))
                {
                    byte[] keyBytes = password.GetBytes(KeySize / 8);

                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.Mode = CipherMode.CBC;
                        using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    using (var reader = new StreamReader(cryptoStream))
                                    {
                                        readerFinal = reader.ReadToEnd();    
                                        return readerFinal;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return readerFinal;
            }
        }
    }
}