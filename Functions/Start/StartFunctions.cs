using System;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class StartFunctions
    {
        static readonly int[] appVersion = [ 0, 0, 0 ];

        public static async void Initialize()
        {
            String[] version = (await new SystemParamDB().GetValue("AppVersion")).Split('.');
            for (int i = 0; i < version.Length; i++)
                appVersion[i] = int.Parse(version[i]);

            version = (await new SystemParamDB().GetValue("BoardVersion")).Split('.');
            for (int i = 0; i < version.Length; i++)
                boardVersion[i] = int.Parse(version[i]);
        }

        // Start
        public static async Task<StartResponse> StartApp(StartRequest request)
        {
            String[] version = request.Version.Split('.');

            for (int i = 0; i < version.Length; i++)
            {
                int intVersion = int.Parse(version[i]);
                if (intVersion > appVersion[i])
                    break;
                if (intVersion < appVersion[i])
                {
                    if (WebEnvConfig.Env == EnvironmentType.DEV)
                        return new StartResponse(0, "0|Heroes Migrantes|Tu App está desactualizada.\r\nPor favor actualízala e intenta de nuevo.",
                                                    "https://drive.google.com/drive/u/0/folders/1DWYN4kU6SwK9fO1YmihQrqDA_frWsBbl|<None>");
                    if (WebEnvConfig.Env == EnvironmentType.PROD)
                        return new StartResponse(0, "0|Heroes Migrantes|Tu App está desactualizada.\r\nPor favor actualízala e intenta de nuevo.",
                                                    "https://drive.google.com/drive/u/0/folders/1gzP8z6hc70tq0aZqM9q4UNDnygokw6OK|<None>");
                                                    //"https://play.google.com/store/apps/details?id=com.Hero.Migrant|https://apps.apple.com/gt/app/hero-migrant/idxxxxxxxxxx");
                    return new StartResponse(0, "0|Heroes Migrantes|¡Tienes que actualizar tu App!");
                }
            }

            String certificates = request.PublicKey == null ? null : await CertificateFunctions.GetSecret(request.PublicKey);

            return new StartResponse(certificates, 1);
        }

        // BOARD

        static readonly int[] boardVersion = [0, 0, 0];

        // Start Board
        public static async Task<StartResponse> StartBoard(StartRequest request)
        {
            String[] version = request.Version.Split('.');

            for (int i = 0; i < version.Length; i++)
            {
                int intVersion = int.Parse(version[i]);
                if (intVersion > boardVersion[i])
                    break;
                if (intVersion < boardVersion[i])
                    return new StartResponse(0, "0|Heroes Migrantes|Tu Board está desactualizado.\r\nPor favor refresca esta página Web e intenta de nuevo.");
            }

            String certificates = request.PublicKey == null ? null : await CertificateFunctions.GetSecret(request.PublicKey);

            return new StartResponse(certificates, 1);
        }
    }
}
