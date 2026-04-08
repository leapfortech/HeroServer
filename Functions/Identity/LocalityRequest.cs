using System;

namespace HeroServer
{
    public class LocalityRequest
    {
        public Locality InterestLocality { get; set; }
        public Locality CurrentLocality { get; set; }
      
        public LocalityRequest()
        {
        }

        public LocalityRequest(Locality interestLocality, Locality currentLocality)
        {
            InterestLocality = interestLocality;
            CurrentLocality = currentLocality;
        }
    }
}