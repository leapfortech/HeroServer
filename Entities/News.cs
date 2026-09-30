using System;

namespace HeroServer
{
    public class News
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long NewsTypeId { get; set; }
        public String Place { get; set; }
        public String Source { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public News() { }

        public News(long id, long postId, long newsTypeId, String place, String source,
                    DateTime? dateTime, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            NewsTypeId = newsTypeId;
            Place = place;
            Source = source;
            DateTime = dateTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public News(NewsFull newFull)
        {
            Id = newFull.Id;
            PostId = newFull.PostId;
            NewsTypeId = newFull.NewsTypeId;
            Place = newFull.Place;
            Source = newFull.Source;
            DateTime = newFull.DateTime;
            CreateDateTime = System.DateTime.Now;
            UpdateDateTime = System.DateTime.Now;
            Status = newFull.Status;
        }
    }
}
