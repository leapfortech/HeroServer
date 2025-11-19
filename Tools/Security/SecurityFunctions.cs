using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using UECDiffieHellman = MLAPI.Cryptography.KeyExchanges.ECDiffieHellman;

namespace HeroServer
{
    public static class SecurityFunctions
    {

        public static long GetUid()
        {
            long uid = -1;
            return (uid);
        }


        public static (byte[], byte[]) GetBobKeys(String alice)
        {
            UECDiffieHellman bob = new UECDiffieHellman();

            byte[] publicBobKey;
            try
            {
                publicBobKey = bob.GetPublicKey();
            }
            catch (Exception ex)
            {
                throw new Exception("B1 : " + ex.Message);
            }

            byte[] commonKey;
            try
            {
                commonKey = bob.GetSharedSecretRaw(Convert.FromBase64String(alice));
            }
            catch (Exception ex)
            {
                throw new Exception("B2 : " + Convert.ToBase64String(publicBobKey) + "\r\n" + ex.Message);
            }

            return (publicBobKey, commonKey);
        }

        public static byte[] ConcatData(params byte[][] arrays)
        {
            int resultLength = 0;
            for (int i = 0; i < arrays.Length; i++)
                resultLength += arrays[i].Length;

            byte[] result = new byte[resultLength];
            int idx = 0;
            for (int i = 0; i < arrays.Length; idx += arrays[i].Length, i++)
                Buffer.BlockCopy(arrays[i], 0, result, idx, arrays[i].Length);

            return result;
        }

        public static String ConcatSizes(params int[] sizes)
        {
            String size = Convert.ToString(sizes.Length, 16);
            String result = Convert.ToString(size.Length, 16) + size;

            for (int i = 0; i < sizes.Length; i++)
            {
                size = Convert.ToString(sizes[i], 16);
                result += Convert.ToString(size.Length, 16) + size;
            }

            String shiftSizes = "";
            for (int i = 0; i < result.Length; i++)
                shiftSizes += (char)(result[i] + 17);

            return shiftSizes;
        }

        public static async Task<(byte[], byte[])> Encrypt(byte[] key, String message)
        {
            try
            {
                using (Aes aes = Aes.Create()) // new AesCryptoServiceProvider()) // NET6
                {
                    aes.Key = key;

                    byte[] encryptedMessage;

                    // Encrypt the message
                    using (MemoryStream ciphertext = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            byte[] plaintextMessage = Encoding.UTF8.GetBytes(message);
                            await cs.WriteAsync(plaintextMessage);
                            cs.Close();
                            encryptedMessage = ciphertext.ToArray();
                        }
                    }

                    return (encryptedMessage, aes.IV);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Convert.ToBase64String(key) + "\r\n" + ex.Message);
            }
        }
    }
}
