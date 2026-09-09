using System;

namespace HeroServer
{
    public class AppUserInfo
    {
        public long Id { get; set; }
        public String Alias { get; set; }
        public String Thumbnail { get; set; }
        public LocalityFull InterestLocality { get; set; }
        public LocalityFull CurrentLocality { get; set; }

        public AppUserInfo()
        {

        }

        public AppUserInfo(long id, String alias, String thumbnail, LocalityFull interestLocality, LocalityFull currentLocality)
        {
            Id = id;
            Alias = alias;
            Thumbnail = thumbnail;
            InterestLocality = interestLocality;
            CurrentLocality = currentLocality;
        }
    }
}
