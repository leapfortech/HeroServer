using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class LoginAppInfo
    {
        public List<NewsInfo> NewsInfos { get; set; }
        public ReferredCount ReferredCount { get; set; }
        public Identity Identity { get; set; }
        public Address Address { get; set; }
        public String Portrait { get; set; }
        public Card Card { get; set; }
        public List<Notification> Notifications { get; set; }

        public LoginAppInfo()
        {
        }

        public LoginAppInfo(List<NewsInfo> newsInfos, ReferredCount referredCount, Identity identity, Address address, String portrait, Card card, List<Notification> notifications)
        {
            NewsInfos = newsInfos;
            ReferredCount = referredCount;
            Identity = identity;
            Address = address;
            Portrait = portrait;
            Card = card;
            Notifications = notifications;
        }
    }
}
