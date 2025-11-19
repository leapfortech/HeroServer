using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class WebSysPinRequest
    {
        public long WebSysUserId { get; set; }
        public String Pin { get; set; }


        public WebSysPinRequest()
        {
        }

        public WebSysPinRequest(long webSysUserId, String pin)
        {
            WebSysUserId = webSysUserId;
            Pin = pin;
        }
    }
}