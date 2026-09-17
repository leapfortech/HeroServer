using System;

namespace HeroServer
{
    public class RadioFeed
    {
        public String TitleImage { get; set; }
        public String Title { get; set; }
        public String RadioType { get; set; }
        public String PostCountry { get; set; }
        public String PostState { get; set; }
        public String Url { get; set; }

        public RadioFeed()
        {

        }

        public RadioFeed(String titleImage, String title, String radioType, String postCountry, String postState, String url)
        {
            TitleImage = titleImage;
            Title = title;
            RadioType = radioType;
            PostCountry = postCountry;
            PostState = postState;
            Url = url;
        }
    }
}
