using System;

namespace HeroServer
{
    public class LoginAppResponse
    {
        public AppUser AppUser { get; set; }
        public WebSysUser WebSysUser { get; set; }
        public int Granted { get; set; }
        public String Message { get; set; }
        public String Link { get; set; }

        public LoginAppResponse()
        {
        }

        public LoginAppResponse(AppUser appUser, WebSysUser webSysUser, int granted, String message = null, String link = null)
        {
            AppUser = appUser;
            WebSysUser = webSysUser;
            Granted = granted;
            Message = message;
            Link = link;
        }
    }
}
