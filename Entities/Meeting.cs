using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class Meeting
    {
        public int Id { get; set; }
        public int BoardUserId { get; set; }
        public int MeetingTypeId { get; set; }
        public String Subject { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public String Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Meeting()
        {
        }

        public Meeting(int id, int boardUserId, int meetingTypeId, String subject, DateTime startDateTime, DateTime endDateTime, String description, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            BoardUserId = boardUserId;
            MeetingTypeId = meetingTypeId;
            Subject = subject;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Description = description;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
