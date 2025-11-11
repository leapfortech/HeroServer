using System;

namespace HeroServer
{
    public class VisionRequest
    {
        public String Image { get; set; }
        public String Image2 { get; set; }
        public bool ExtractFace { get; set; }

        public VisionRequest()
        {
            Image = null;
            Image2 = null;
            ExtractFace = false;
        }

        public VisionRequest(String image)
        {
            Image = image;
            Image2 = null;
            ExtractFace = true;
        }

        public VisionRequest(String image, String image2, bool extractPortrait = true)
        {
            Image = image;
            Image2 = image2;
            ExtractFace = extractPortrait;
        }
    }
}
