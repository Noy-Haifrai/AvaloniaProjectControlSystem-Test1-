using System;
using System.Security.Cryptography;
using System.Text;

namespace AvaloniaTest1.Classes
{
    //TripleDES шифрование
    class tDES
    {
        // Шифрует строку и возвращает результат в Base64
        public string Encrypt(string toEncrypt, string key)
        {
            using (var md5 = MD5.Create())
            using (var tdes = new TripleDESCryptoServiceProvider())
            {
                var utf8 = new UTF8Encoding();
                tdes.Key = md5.ComputeHash(utf8.GetBytes(key));
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                using (var encryptor = tdes.CreateEncryptor())
                {
                    byte[] bytesToEncrypt = utf8.GetBytes(toEncrypt);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }
        // Дешифрует Base64 строку и возвращает исходный текст
        public string Decrypt(string toDecrypt, string key)
        {
            if (string.IsNullOrEmpty(toDecrypt))
                throw new ArgumentNullException(nameof(toDecrypt));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            using (var md5 = MD5.Create())
            using (var tdes = new TripleDESCryptoServiceProvider())
            {
                var utf8 = new UTF8Encoding();
                tdes.Key = md5.ComputeHash(utf8.GetBytes(key));
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                using (var decryptor = tdes.CreateDecryptor())
                {
                    byte[] encryptedBytes = Convert.FromBase64String(toDecrypt);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return utf8.GetString(decryptedBytes);
                }
            }
        }
    }
}

