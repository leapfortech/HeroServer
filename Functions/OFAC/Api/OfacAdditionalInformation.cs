using System;

namespace HeroServer
{
    public class OfacAdditionalInformation
    {
#pragma warning disable IDE1006 // Naming Styles
        public String label { get; set; }
        public String value { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacAdditionalInformation()
        {
        }

        public OfacAdditionalInformation(String label, String value)
        {
            this.label = label;
            this.value = value;
        }
    }
}
