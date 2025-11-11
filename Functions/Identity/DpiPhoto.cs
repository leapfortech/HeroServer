using System;

namespace HeroServer
{
    public class DpiPhoto
    {
        public String DpiFront { get; set; }
        public String DpiBack { get; set; }
        public String DpiPortrait { get; set; }

        public DpiPhoto()
        {
        }

        public DpiPhoto(String dpiFront, String dpiBack, String dpiPortrait)
        {
            DpiFront = dpiFront;
            DpiBack = dpiBack;
            DpiPortrait = dpiPortrait;
        }
    }
}
