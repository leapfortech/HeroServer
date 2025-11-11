using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SysEnvironment = System.Environment;
using Microsoft.Extensions.Configuration;

namespace HeroServer
{
    public static class WebEnvConfig
    {
        static readonly Dictionary<EnvironmentType, Environment> environments = new Dictionary<EnvironmentType, Environment>()
        {
            { EnvironmentType.DEV,     new Environment("KeyVault:VaultUriDev",    "heroDbDev",    "heroAkDev",    "[S-SystemParam]") },
            { EnvironmentType.QA,      new Environment( "KeyVault:VaultUriQa",     "heroDbQa",     "heroAkQa",    "[S-SystemParam]") },
            { EnvironmentType.PROD,    new Environment(   "KeyVault:VaultUri",       "heroDb",       "heroAk",    "[S-SystemParam]") }
        };

        static EnvironmentType env = EnvironmentType.NONE;
        static String connString = null;
        static String storageString = null;
        static String firebase = null;

        public static async Task<KeyVaultHelper> Initialize(IConfiguration configuration)
        {
            String envFlag = SysEnvironment.GetEnvironmentVariable("APP_ENVIRONMENT") ??
                throw new Exception("ERROR : No App Environment Defined");

            if (!Enum.TryParse(envFlag, out env))
                throw new Exception($"ERROR : Unknown App Environment {envFlag}");

            if (env == EnvironmentType.NONE)
                throw new Exception($"ERROR : Inactive App Environment {envFlag}");

            KeyVaultHelper keyVaultHelper = new KeyVaultHelper(configuration.GetValue<String>(KeyVault));

            connString = await keyVaultHelper.GetSecret(environments[env].Db);
            storageString = await keyVaultHelper.GetSecret(environments[env].Storage);

            firebase = await new SystemParamDB().GetValue("Firebase");

            return keyVaultHelper;
        }

        public static EnvironmentType Env => env;

        public static String ConnString => connString;
        public static String StorageString => storageString;
        public static String Flag => env.ToString();
        public static String KeyVault => environments[env].KeyVault;
        public static String Db => environments[env].Db;
        public static String Storage => environments[env].Storage;
        public static String Params => environments[env].Params;
        public static String Firebase => firebase;
    }

    public enum EnvironmentType { NONE, DEV, QA, PROD }

    public class Environment(String keyVault, String db, String storage, String prms)
    {
        public String KeyVault = keyVault;
        public String Db = db;
        public String Storage = storage;
        public String Params = prms;
    }
}