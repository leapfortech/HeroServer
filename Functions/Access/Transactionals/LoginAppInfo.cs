using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class LoginAppInfo
    {
        public ReferredCount ReferredCount { get; set; }
        public Identity Identity { get; set; }
        public Address Address { get; set; }
        public String Portrait { get; set; }
        public Locality InterestLocality { get; set; }
        public Locality CurrentLocality { get; set; }
        public Player Player { get; set; }
        public List<PuzzleResultSummary> PuzzleResultSummarys { get; set; }
        public Card Card { get; set; }
        public List<Notification> Notifications { get; set; }


        public LoginAppInfo()
        {
        }

        public LoginAppInfo(ReferredCount referredCount, Identity identity, Address address, String portrait,
                            Locality interestLocality, Locality currentLocality, Player player, List<PuzzleResultSummary> puzzleResultSummarys,
                            Card card, List<Notification> notifications)
        {
            ReferredCount = referredCount;
            Identity = identity;
            Address = address;
            Portrait = portrait;
            Card = card;
            Player = player;
            PuzzleResultSummarys = puzzleResultSummarys;
            InterestLocality = interestLocality;
            CurrentLocality = currentLocality;
            Notifications = notifications;
        }
    }
}
