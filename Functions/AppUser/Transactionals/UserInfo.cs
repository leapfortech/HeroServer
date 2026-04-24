using System;

namespace HeroServer
{
    public class UserInfo
    {
        public AppUserFull AppUserFull { get; set; }
        public IdentityFull IdentityFull { get; set; }
      
        public UserInfo()
        {
        }

        public UserInfo(AppUserFull appUserFull, IdentityFull identityFull)
        {
            AppUserFull = appUserFull;
            IdentityFull = identityFull;
        }
    }
}