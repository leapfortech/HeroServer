using System;

namespace HeroServer
{
    public class News
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long NewsTypeId { get; set; }
        public long OriginCountryId { get; set; }
        public long OriginStateId { get; set; }
        public String NewsSource { get; set; }
        public String NewsUrl { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public News() { }

        public News(long id, long postId, long newsTypeId, long originCountryId, long originStateId, String newsSource,
                    String newsUrl, DateTime? dateTime, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            NewsTypeId = newsTypeId;
            OriginCountryId = originCountryId;
            OriginStateId = originStateId;
            NewsSource = newsSource;
            NewsUrl = newsUrl;
            DateTime = dateTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
