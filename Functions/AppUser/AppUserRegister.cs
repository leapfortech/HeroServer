using System;

namespace HeroServer
{
    public class AppUserRegister
    {
        public AppUser AppUser { get; set; }
        public String DpiFront { get; set; }
        public String DpiBack { get; set; }
        public String Portrait { get; set; }
        public String DpiPortrait { get; set; }

        public AppUserRegister()
        {
        }

        public AppUserRegister(AppUser appUser, String dpiFront = null, String dpiBack = null, String portrait = null, String dpiPortrait = null)
        {
            AppUser = appUser;
            DpiFront = dpiFront;
            DpiBack = dpiBack;
            Portrait = portrait;
            DpiPortrait = dpiPortrait;
        }
    }
}
