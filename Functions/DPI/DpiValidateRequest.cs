using System;

namespace HeroServer
{
    public class DpiValidateRequest
    {
        public Identity Identity { get; set; }
        public String MRZ { get; set; }

        public Dpi DPI { get; set; }


        public DpiValidateRequest()
        {
        }

        public DpiValidateRequest(Identity identity, String mrz, Dpi dpi)
        {
            Identity = identity;
            MRZ = mrz;
            DPI = dpi;
        }
    }
}
