using System;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace HeroServer
{
    public class KeyVaultHelper(String url) : IKeyVaultHelper
    {
        private readonly SecretClient secretClient = new SecretClient(new Uri(url), new DefaultAzureCredential());

        public async Task<String> GetSecret(String secretName)
        {
            KeyVaultSecret keyVaultSecret = await secretClient.GetSecretAsync(secretName);
            return keyVaultSecret.Value;
        }
    }
}