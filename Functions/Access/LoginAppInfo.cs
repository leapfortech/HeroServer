using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class LoginAppInfo
    {
        public List<NewsInfo> NewsInfos { get; set; }
        public List<MeetingFull> MeetingFulls { get; set; }
        public ReferredCount ReferredCount { get; set; }
        public Identity Identity { get; set; }
        public Address Address { get; set; }
        public String Portrait { get; set; }
        public Card Card { get; set; }
        public List<Notification> Notifications { get; set; }

        public List<ProjectProductFull> ProjectProductFulls { get; set; }

        public List<InvestmentFractionatedFull> InvestmentFractionatedFulls { get; set; }
        public List<InvestmentFinancedFull> InvestmentFinancedFulls { get; set; }
        public List<InvestmentPrepaidFull> InvestmentPrepaidFulls { get; set; }

        public List<int> ProjectLikeIds { get; set; }

        public LoginAppInfo()
        {
        }

        public LoginAppInfo(List<NewsInfo> newsInfos, List<MeetingFull> meetingFulls, ReferredCount referredCount, Identity identity, Address address, String portrait, Card card, List<Notification> notifications,
                            List<ProjectProductFull> projectProductFulls,
                            List<InvestmentFractionatedFull> investmentFractionatedFulls,
                            List<InvestmentFinancedFull> investmentFinancedFulls,
                            List<InvestmentPrepaidFull> investmentPrepaidFulls,
                            List<int> projectLikeIds)
        {
            NewsInfos = newsInfos;
            MeetingFulls = meetingFulls;
            ReferredCount = referredCount;
            Identity = identity;
            Address = address;
            Portrait = portrait;
            Card = card;
            Notifications = notifications;

            ProjectProductFulls = projectProductFulls;

            InvestmentFractionatedFulls = investmentFractionatedFulls;
            InvestmentFinancedFulls = investmentFinancedFulls;
            InvestmentPrepaidFulls = investmentPrepaidFulls;

            ProjectLikeIds = projectLikeIds;
        }
    }
}
