using System;
using System.Threading.Tasks;

namespace HeroServer
{
    public interface IKeyVaultHelper
    {
        public Task<String> GetSecret(String secretName);
    }
}
