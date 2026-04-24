using System;

namespace HeroServer
{
    public class UserInfo
    {
        public AppUserFull AppUserFull { get; set; }
        public IdentityFull IdentityFull { get; set; }
        public AddressFull AddressFull { get; set; }

        public UserInfo()
        {
        }

        public UserInfo(AppUserFull appUserFull, IdentityFull identityFull, AddressFull addressFull)
        {
            AppUserFull = appUserFull;
            IdentityFull = identityFull;
            AddressFull = addressFull;
        }
    }
}