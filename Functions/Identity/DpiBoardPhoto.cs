using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class DpiBoardPhoto
    {
        public List<String> DpiFronts { get; set; }
        public List<String> DpiBacks { get; set; }
        public String DpiPortrait { get; set; }

        public DpiBoardPhoto()
        {
        }

        public DpiBoardPhoto(List<String> dpiFronts, List<String> dpiBacks, String dpiPortrait)
        {
            DpiFronts = dpiFronts;
            DpiBacks = dpiBacks;
            DpiPortrait = dpiPortrait;
        }
    }
}
