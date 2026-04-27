
using System;

namespace HeroServer
{
    public class PostFeedRequest
    {
        // PARAMS
        public int Count { get; set; } = 20;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        // FILTERS
        public long AppUserId { get; set; } = -1;
        public long PostTypeId { get; set; } = -1;
        public long CountryId { get; set; } = -1;
        public long StateId { get; set; } = -1;
        public int Status { get; set; } = -1;
    }
}
