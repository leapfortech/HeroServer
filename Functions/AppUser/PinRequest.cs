using System;

namespace HeroServer
{
    public class PinRequest
    {
        public int AppUserId { get; set; }
        public String Pin { get; set; }


        public PinRequest()
        {
        }

        public PinRequest(int appUserId, String pin)
        {
            AppUserId = appUserId;
            Pin = pin;
        }
    }
}
