using System;

namespace HeroServer
{
    public class LeapRequest
    {
        public LeapData Data { get; set; }

        public LeapRequest()
        {
        }

        public LeapRequest(LeapData data)
        {
            Data = data;
        }
    }
}
