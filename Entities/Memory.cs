using System;

namespace HeroServer
{
    public class Memory
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long MemoryTypeId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public DateTime? DateTime { get; set; }
        public String Location { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Memory() { }

        public Memory(long id, long postId, long memoryTypeId, long countryId, long stateId,
                      DateTime? dateTime, String location, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            MemoryTypeId = memoryTypeId;
            CountryId = countryId;
            StateId = stateId;
            DateTime = dateTime;
            Location = location;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
