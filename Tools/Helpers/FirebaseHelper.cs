using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using FirebaseAdmin.Messaging;
using FBNotification = FirebaseAdmin.Messaging.Notification;

namespace HeroServer
{
    public static class FirebaseHelper
    {
        public static async Task<int> SendMessage(int appUserId, String dataName, int dataId, String title, String body,
                                                  String action, String information, String parameter, int displayMode, ILogger logger)
        {
            long webSysUserId = await new AppUserDB().GetWebSysUserId(appUserId);
            (String firstName1, String _1, String lastName1, String _2) = await new IdentityDB().GetFullNameByAppUserId(appUserId);

            String name;
            if (firstName1 == null)
            {
                logger?.LogWarning("WARNING : No Name On {DataName} #{DataId} to AppUser #{AppUserId}", dataName, dataId, webSysUserId);
                String eMail = await new WebSysUserDB().GetEmailById(webSysUserId);
                name = eMail[..eMail.IndexOf('@')];
            }
            else
                name = firstName1 + " " + lastName1;

            List<WebSysToken> webSysTokens = await new WebSysTokenDB().GetByWebSysUserId(webSysUserId, 1);
            if (webSysTokens.Count == 0)
            {
                logger?.LogWarning("ERROR : No Token On {DataName} #{DataId} to WebSysUser #{WebSysUserId}", dataName, dataId, webSysUserId);
                return 0;
            }

            //logger?.LogWarning("INFO : Sending {DataName} #{DataId} to WebSysUser #{WebSysUserId} | Token #{webSysTokenId} ({webSysTokenCount})", dataName, dataId, appUser.WebSysUserId, webSysTokens[0].Id, webSysTokens.Count);

            String[] tokens = new String[webSysTokens.Count];
            for (int i = 0; i < tokens.Length; i++)
                tokens[i] = webSysTokens[i].Token;

            body = name + body;

            MulticastMessage message = new MulticastMessage()
            {
                Tokens = tokens,
                Notification = new FBNotification()
                {
                    Title = title,
                    Body = body,
                },
                Data = new Dictionary<String, String>()
                {
                    { "WebSysUserId", webSysUserId.ToString() },
                    { "Action", action },
                    { "Information", information },
                    { "Parameter", parameter },
                    { "DisplayMode", displayMode.ToString() }
                }
            };

            // DB
            Notification notification = new Notification(-1, webSysUserId, null, title, body, action, information, parameter, displayMode, DateTime.Now, 1);
            int notificationId = await new NotificationDB().Add(notification);

            // Send
            //logger?.LogWarning("INFO : SendMessage On {DataName} #{DataId} to AppUser #{AppUserId}", dataName, dataId, appUserId);

            BatchResponse batchResponse = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

            //logger?.LogWarning("INFO : Successes {Successes} | Fails {Fails}", batchResponse.SuccessCount, batchResponse.FailureCount);

            if (batchResponse.FailureCount == batchResponse.Responses.Count)
            {
                logger?.LogError("ERROR : Cannot SendMessage On {DataName} #{DataId} with NO Token", dataName, dataId);
                await new NotificationDB().UpdateMessageId(notificationId, "NONE", 2);
                return 2;
            }

            int successIdx = -1;
            for (int i = 0; i < batchResponse.Responses.Count; i++)
            {
                if (batchResponse.Responses[i].IsSuccess)
                {
                    if (successIdx == -1)
                        successIdx = i;
                    continue;
                }

                logger?.LogWarning("WARNING : [{ErrorCode}] SendMessage On {DataName} #{DataId} with Token #{Token}", batchResponse.Responses[i].Exception.ErrorCode.ToString(), dataName, dataId, message.Tokens[i]);

                if (batchResponse.Responses[i].Exception.ErrorCode == FirebaseAdmin.ErrorCode.NotFound)
                {
                    int idx = await new WebSysTokenDB().Find(new WebSysToken(-1, webSysUserId, message.Tokens[i], 1));
                    if (idx != -1)
                    {
                        //logger?.LogWarning("INFO : Disabling Token #{TokenIdx}", idx);
                        await new WebSysTokenDB().UpdateStatus(idx, 0);
                    }
                }
            }

            // Result
            //logger?.LogWarning("INFO : Updating Notification #{NotificationId}", notificationId);
            await new NotificationDB().UpdateMessageId(notificationId, WebHelper.TrimNotifMsgId(batchResponse.Responses[successIdx].MessageId), 1);
            return 1;
        }
    }
}
