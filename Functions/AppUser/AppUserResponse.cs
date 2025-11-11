using System;

namespace HeroServer
{
    public class AppUserResponse
    {
        public AppUser AppUser { get; set; }
        public String IdDocFront { get; set; }
        public String IdDocBack { get; set; }

        public AppUserResponse()
        {
        }

        public AppUserResponse(AppUser appUser, String idDocFront = null, String idDocBack = null)
        {
            AppUser = appUser;
            IdDocFront = idDocFront;
            IdDocBack = idDocBack;
        }
    }
}
