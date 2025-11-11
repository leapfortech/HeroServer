using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using MLAPI.Cryptography.Utils;

namespace MLAPI.Cryptography.KeyExchanges
{
    public class ECDiffieHellmanRSA
    {
        private readonly ECDiffieHellman diffieHellman = new ECDiffieHellman();
        private readonly RSA rsa;

        private bool IsSigner => rsa?.ExportParameters(true).D != null; // .PublicOnly;  // NET6

        public ECDiffieHellmanRSA(X509Certificate2 certificate)
        {
            if (certificate.HasPrivateKey)
                rsa = certificate.GetRSAPrivateKey();
            else
                rsa = certificate.GetRSAPublicKey();

            if (rsa == null)
                throw new CryptographicException("Only RSA certificates are supported. No valid RSA key was found");
        }

        public ECDiffieHellmanRSA(RSA rsa)
        {
            this.rsa = rsa;

            if (this.rsa == null)
                throw new CryptographicException("Key cannot be null");
        }

        public byte[] GetSecurePublicPart()
        {
            byte[] publicPart = diffieHellman.GetPublicKey();

            using (SHA256 sha = SHA256.Create())
            {
                byte[] proofPart;

                if (IsSigner)
                {
                    // Sign the hash with the private key
                    proofPart = rsa.SignData(publicPart, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);  // NET6
                }
                else
                {
                    // Encrypt the public part with the opposite public
                    byte[] realHash = sha.ComputeHash(publicPart);  // NET6
                    proofPart = rsa.Encrypt(realHash, RSAEncryptionPadding.Pkcs1);  // NET6
                }

                // Final has two lengths appended
                byte[] final = new byte[(sizeof(ushort) * 2) + publicPart.Length + proofPart.Length];

                // Write lengths to final
                for (byte i = 0; i < sizeof(ushort); i++) final[i] = ((byte)(publicPart.Length >> (i * 8)));
                for (byte i = 0; i < sizeof(ushort); i++) final[i + sizeof(ushort)] = ((byte)(proofPart.Length >> (i * 8)));

                // Copy parts
                Buffer.BlockCopy(publicPart, 0, final, (sizeof(ushort) * 2), publicPart.Length);
                Buffer.BlockCopy(proofPart, 0, final, (sizeof(ushort) * 2) + publicPart.Length, proofPart.Length);

                return final;
            }
        }

        public byte[] GetVerifiedSharedPart(byte[] securePart)
        {
            if (securePart.Length < 4)
                throw new ArgumentException("Signed part was too short");

            // Read lengths
            ushort publicLength = (ushort)(((ushort)securePart[0]) | ((ushort)securePart[1] << 8));
            ushort proofLength = (ushort)(((ushort)securePart[2]) | ((ushort)securePart[3] << 8));

            if (securePart.Length != publicLength + proofLength + (sizeof(ushort) * 2))
                throw new CryptographicException("Part lengths did not match");

            // Alloc parts
            byte[] publicPart = new byte[publicLength];
            byte[] proofPart = new byte[proofLength];

            // Copy parts
            Buffer.BlockCopy(securePart, sizeof(ushort) * 2, publicPart, 0, publicLength);
            Buffer.BlockCopy(securePart, sizeof(ushort) * 2 + publicLength, proofPart, 0, proofLength);

            if (IsSigner)
            {
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] claimedHash = rsa.Decrypt(proofPart, RSAEncryptionPadding.Pkcs1);  // NET6
                    byte[] realHash = sha.ComputeHash(publicPart);

                    // Prevent timing attacks by using constant time
                    if (!ComparisonUtils.ConstTimeArrayEqual(claimedHash, realHash))
                        throw new CryptographicException("Hash did not match the signed hash");

                    return diffieHellman.GetSharedSecretRaw(publicPart);
                }
            }
            else
            {
                using (SHA256 sha = SHA256.Create())
                {
                    if (!rsa.VerifyData(publicPart, proofPart, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                        throw new CryptographicException("Signature was invalid");

                    return diffieHellman.GetSharedSecretRaw(publicPart);
                }
            }
        }
    }
}
