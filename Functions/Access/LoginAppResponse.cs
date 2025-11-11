using System;

namespace HeroServer
{
    public class LoginAppResponse
    {
        public AppUser AppUser { get; set; }
        public WebSysUser WebSysUser { get; set; }
        public int Granted { get; set; }
        public int OnboardingStage { get; set; }
        public String Message { get; set; }
        public String Link { get; set; }

        public LoginAppResponse()
        {
        }

        public LoginAppResponse(AppUser appUser, WebSysUser webSysUser, int granted, int onboardingStage, String message = null, String link = null)
        {
            AppUser = appUser;
            WebSysUser = webSysUser;
            Granted = granted;
            OnboardingStage = onboardingStage;
            Message = message;
            Link = link;
        }

        public LoginAppResponse(AppUser appUser, WebSysUser webSysUser, int granted, String message = null, String link = null)
        {
            AppUser = appUser;
            WebSysUser = webSysUser;
            Granted = granted;
            OnboardingStage = 0;
            Message = message;
            Link = link;
        }
    }
}
