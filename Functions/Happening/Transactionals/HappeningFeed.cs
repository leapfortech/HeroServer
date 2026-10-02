using System;

namespace HeroServer
{
    public class HappeningFeed
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String TitleImage { get; set; }
        public String Title { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public String HappeningType { get; set; }
        public String Country { get; set; }
        public String State { get; set; }
        public String Location { get; set; }
        public int Favorite { get; set; }
        public int Selected { get; set; }
        public int FavoriteCount { get; set; }
        public DateTime PublicationDateTime { get; set; }

        public HappeningFeed()
        {

        }

        public HappeningFeed(long id, long postId, String titleImage, String title, DateTime? startDateTime, DateTime? endDateTime,
                             String happeningType, String country, String state, String location, int favorite, int selected, int favoriteCount, DateTime publicationDateTime)
        {
            Id = id;
            PostId = postId;
            TitleImage = titleImage;
            Title = title;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            HappeningType = happeningType;
            Country = country;
            State = state;
            Location = location;
            Favorite = favorite;
            Selected = selected;
            FavoriteCount = favoriteCount;
            PublicationDateTime = publicationDateTime;
        }
    }
}

