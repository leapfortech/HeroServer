using System;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class StartFunctions
    {
        static readonly int[] appVersion = [ 0, 4, 3 ];

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
                                                    "https://drive.google.com/drive/u/0/folders/1TOjpOilO2U4XvCRaNc1KPaBwKyet96P0|<None>");
                    if (WebEnvConfig.Env == EnvironmentType.PROD)
                        return new StartResponse(0, "0|Heroes Migrantes|Tu App está desactualizada.\\r\\nPor favor actualízala e intenta de nuevo.",
                                                    "https://drive.google.com/drive/u/0/folders/1oTH1HSUGFgjwgq3CujaBu6kha5eV77so|<None>");
                                                    //"https://play.google.com/store/apps/details?id=com.Hpb.Expande|https://apps.apple.com/gt/app/expande/id6745855017");
                    return new StartResponse(0, "0|Expande|¡Tienes que actualizar tu App!");
                }
            }

            String certificates = await CertificateFunctions.GetSecret(request.PublicKey);

            return new StartResponse(certificates, 1);
        }

        // BOARD

        static readonly int[] boardVersion = [0, 1, 0];

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
                    return new StartResponse(0, "0|Heroes Migrantes|Tu Board está desactualizado.\r\nPor favor actualízalo e intenta de nuevo.");
            }

            String certificates = await CertificateFunctions.GetSecret(request.PublicKey);

            return new StartResponse(certificates, 1);
        }
    }
}
