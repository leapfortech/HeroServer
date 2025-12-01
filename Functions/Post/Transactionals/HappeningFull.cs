using System;

namespace HpbServer
{
    public class HappeningFull : PostFull
    {
        public long Id { get; set; }
        public long EventTypeId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public String Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Status { get; set; }

        public HappeningFull()
        {
        }

        public HappeningFull(long id, long postId, long appUserId, String appUserAlias,
                             long postTypeId, long postSubtypeId,
                             long postOriginCountryId, long postOriginStateId,
                             String title, String summary, String description,
                             int imageCount, int likesCount, DateTime publicationDateTime,
                             int postStatus,
                             long eventTypeId, long countryId, long stateId,
                             DateTime? startDateTime, DateTime? endDateTime,
                             String location, double? latitude, double? longitude,
                             int status)
            : base(postId, appUserId, appUserAlias, postTypeId, postSubtypeId,
                   postOriginCountryId, postOriginStateId, title, summary, description,
                   imageCount, likesCount, publicationDateTime, postStatus)
        {
            Id = id;
            EventTypeId = eventTypeId;
            CountryId = countryId;
            StateId = stateId;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Location = location;
            Latitude = latitude;
            Longitude = longitude;
            Status = status;
        }
    }
}

