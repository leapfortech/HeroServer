
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

        // CURSOR
        public string Cursor { get; set; }

        public int Direction { get; set; } = 0;
    }
}
