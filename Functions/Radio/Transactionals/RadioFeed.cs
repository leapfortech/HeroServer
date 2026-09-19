using System;

namespace HeroServer
{
    public class RadioFeed
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String TitleImage { get; set; }
        public String Title { get; set; }
        public String RadioType { get; set; }
        public String PostCountry { get; set; }
        public String PostState { get; set; }
        public String Url { get; set; }
        public DateTime PublicationDateTime { get; set; }

        public RadioFeed()
        {

        }

        public RadioFeed(long id, long postId, String titleImage, String title, String radioType, String postCountry, String postState, String url, DateTime publicationDateTime)
        {
            Id = id;
            PostId = postId;
            TitleImage = titleImage;
            Title = title;
            RadioType = radioType;
            PostCountry = postCountry;
            PostState = postState;
            Url = url;
            PublicationDateTime = publicationDateTime;
        }
    }
}
