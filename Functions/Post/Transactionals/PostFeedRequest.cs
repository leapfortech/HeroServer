
using System;

namespace HeroServer
{
    public class PostFeedRequest
    {
        // PARAMS
        public int PageSize { get; set; } = 10;

        // FILTERS
        public long AppUserId { get; set; } = -1;
        public long PostSubtypeId { get; set; } = -1;
        public long CountryId { get; set; } = -1;
        public long StateId { get; set; } = -1;
        public int Status { get; set; } = -1;

        // CURSORS
        public DateTime? FirstPublicationDateTime { get; set; }
        public long FirstPostId { get; set; } = -1;

        public DateTime? LastPublicationDateTime { get; set; } = null;
        public long LastPostId { get; set; } = -1;

        public int Direction { get; set; } = 0;
    }
}
