using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace HeroServer
{
    public static class AesHelper
    {
        static Aes aes = null;
        static double aesDelay = 30.0;
        public static double AesDelay => aesDelay;

        public static Aes Aes => aes;

        public static async void Initialize()
        {
            byte[] key = Convert.FromBase64String(await new SystemParamDB().GetValue("AesKey"));
            byte[] iv = Convert.FromBase64String(await new SystemParamDB().GetValue("AesIV"));
            aes = CreateAes(key, iv);

            aesDelay = Convert.ToDouble(await new SystemParamDB().GetValue("AesDelay"));
        }

        public static async void GenerateKey()
        {
            aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.ISO10126;
            aes.Key = new byte[32];
            RandomNumberGenerator.Fill(aes.Key);

            await new SystemParamDB().SetValue("AesKey", Convert.ToBase64String(aes.Key));
            await new SystemParamDB().SetValue("AesIV", Convert.ToBase64String(aes.IV));
        }

        private static Aes CreateAes(byte[] key, byte[] iv)
        {
            Aes cipher = Aes.Create();
            cipher.Mode = CipherMode.CBC;
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = key;
            cipher.IV = iv;
            return cipher;
        }

        public static String Encrypt(String text)
        {
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            return BaseHelper.BytesToBase62(aes.CreateEncryptor().TransformFinalBlock(plaintext, 0, plaintext.Length));
        }

        public static String Decrypt(String encrypted)
        {
            byte[] encryptedBytes = BaseHelper.Base62ToBytes(encrypted);
            if (encryptedBytes.Length < 32)
            {
                byte[] littleBytes = encryptedBytes;
                encryptedBytes = new byte[32];
                littleBytes.CopyTo(encryptedBytes, 32 - littleBytes.Length);
            }
            return Encoding.UTF8.GetString(aes.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
        }

        public static CryptoStream EncryptStream(Stream responseStream)
        {
            ToBase64Transform base64Transform = new ToBase64Transform();
            CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);

            return cryptoStream;
        }


        public static Stream DecryptStream(Stream cipherStream)
        {
            FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
            CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
            return decryptedStream;
        }
    }
}