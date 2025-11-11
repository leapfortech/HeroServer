using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class WebSysPinRequest
    {
        public int WebSysUserId { get; set; }
        public String Pin { get; set; }


        public WebSysPinRequest()
        {
        }

        public WebSysPinRequest(int webSysUserId, String pin)
        {
            WebSysUserId = webSysUserId;
            Pin = pin;
        }
    }
}