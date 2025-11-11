using System;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace HeroServer
{
    public static class CertificateFunctions
    {
        static String azureUrl = "";
        static String cybersourceUrl = "";

        static String azureCert = "";
        static String cybersourceCert = "";

        // Certificate
        public static async void Initialize()
        {
            azureUrl = await new SystemParamDB().GetValue("AzureServerUrl");
            cybersourceUrl = await new SystemParamDB().GetValue("CybersourceServerUrl");

            if (WebEnvConfig.Env == EnvironmentType.DEV)
                return;

            azureCert = await new SystemParamDB().GetValue("AzureCertificate");
        }

        private static async Task GetAzureCertificate()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = AzureCertifValidation
            };

            await new HttpClient(handler).GetAsync(azureUrl);
        }

        private static bool AzureCertifValidation(HttpRequestMessage requestMessage, X509Certificate2 cert2, X509Chain chain, SslPolicyErrors errors)
        {
            //azureCert = cert2.GetPublicKeyString();
            azureCert = cert2.Issuer + "|" + cert2.Subject + "|" + cert2.Thumbprint;
            return true;
        }

        private static async Task GetCybersourceCertificate()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = CybersourceCertifValidation
            };

            await new HttpClient(handler).GetAsync(cybersourceUrl);
        }

        private static bool CybersourceCertifValidation(HttpRequestMessage requestMessage, X509Certificate2 cert2, X509Chain chain, SslPolicyErrors errors)
        {
            //cybersourceCert = cert2.GetPublicKeyString();
            cybersourceCert = cert2.Issuer + "|" + cert2.Subject + "|" + cert2.Thumbprint;
            return true;
        }

        //private static string ByteArrayToString(byte[] ba)
        //{
        //    StringBuilder hex = new StringBuilder(ba.Length * 2);
        //    foreach (byte b in ba)
        //        hex.AppendFormat("{0:x2}", b);
        //    return hex.ToString();
        //}

        // Secret
        public static async Task<String> GetSecret(String alice)
        {
            // A
            try
            {
                if (WebEnvConfig.Env == EnvironmentType.DEV)
                {
                    await GetAzureCertificate();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("A0 : " + azureUrl + "\n" + ex.Message);
            }

            try
            {
                await GetCybersourceCertificate();
            }
            catch (Exception ex)
            {
                if (cybersourceCert.Length == 0)
                    throw new Exception("A1 : " + ex.Message);
            }

            // B
            (byte[] bobPublicKey, byte[] commonKey) b;
            do
            {
                b = SecurityFunctions.GetBobKeys(alice);
                //if (b.commonKey.Length == 32)
                //    logger?.LogWarning("PK > OK {CommonKey}", ByteArrayToString(b.commonKey));
                //else
                //    logger?.LogWarning("PK > KO {CommonKey} ({CommonKeyLength})", ByteArrayToString(b.commonKey), b.commonKey.Length);
            }
            while (b.commonKey.Length != 32);

            // C
            (byte[] encAzureCert, byte[] azureIV) c0;
            try
            {
                c0 = await SecurityFunctions.Encrypt(b.commonKey, azureCert);
            }
            catch (Exception ex)
            {
                throw new Exception("C0 : " + ex.Message);
            }

            (byte[] encCybersourceCert, byte[] cybersourceIV) c1;
            try
            {
                c1 = await SecurityFunctions.Encrypt(b.commonKey, cybersourceCert);
            }
            catch (Exception ex)
            {
                throw new Exception("C1 : " + ex.Message);
            }

            byte[] data = SecurityFunctions.ConcatData(b.bobPublicKey, c0.encAzureCert, c0.azureIV, c1.encCybersourceCert, c1.cybersourceIV);
            String sizes = SecurityFunctions.ConcatSizes(b.bobPublicKey.Length, c0.encAzureCert.Length, c0.azureIV.Length, c1.encCybersourceCert.Length, c1.cybersourceIV.Length);

            return sizes + Convert.ToBase64String(data);
        }
    }
}
