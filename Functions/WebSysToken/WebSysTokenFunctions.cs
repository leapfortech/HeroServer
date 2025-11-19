using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FirebaseAdmin.Messaging;

namespace HeroServer
{
    public static class WebSysTokenFunctions
    {
        public static async Task<long> FindAdd(WebSysToken webSysToken)
        {
            long tokenId = -1;

            List<WebSysToken> webSysTokens = await new WebSysTokenDB().GetByWebSysUserId(webSysToken.WebSysUserId);
            for (int i = 0; i < webSysTokens.Count; i++)
            {
                if (webSysTokens[i].Token == webSysToken.Token)
                {
                    tokenId = webSysTokens[i].Id;
                    if (webSysTokens[i].Status != 1)
                        await new WebSysTokenDB().UpdateStatus(webSysTokens[i].Id, 1);
                    continue;
                }

                if (webSysTokens[i].Status != 1)
                    continue;

                Message message = new Message()
                {
                    Token = webSysTokens[i].Token,
                    Data = new Dictionary<String, String>()
                    {
                        { "WebSysUserId", webSysTokens[i].WebSysUserId.ToString() },
                        { "Action", "RemoteLogin" },
                        { "Information", "Nueva sesión^Se ha iniciado sesión en otro dispositivo, por lo que esta sesión se cerrará." },
                        { "DisplayMode", "1" }
                    }
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message).ContinueWith((task) => { });
                await new WebSysTokenDB().UpdateStatus(webSysTokens[i].Id, 0);
            }

            if (tokenId != -1)
                return tokenId;

            return await new WebSysTokenDB().Add(webSysToken);
        }
    }
}
