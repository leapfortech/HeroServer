using System;

namespace HeroServer
{
    public class Happening
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long EventTypeId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public String Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Happening() { }

        public Happening(long id, long postId, long eventTypeId, long countryId, long stateId,
                         DateTime? startDateTime, DateTime? endDateTime, String location, double? latitude,
                         double? longitude, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            EventTypeId = eventTypeId;
            CountryId = countryId;
            StateId = stateId;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Location = location;
            Latitude = latitude;
            Longitude = longitude;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
