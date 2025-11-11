using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class MeetingInfo
    {
        public int Id { get; set; }
        public int BoardUserId { get; set; }
        public int MeetingTypeId { get; set; }
        public String Subject { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public String Description { get; set; }
        public int Status { get; set; }
        public List<String> Appointments { get; set; }

        public MeetingInfo()
        {
        }

        public MeetingInfo(int id, int boardUserId, int meetingTypeId, String subject, DateTime startDateTime, DateTime endDateTime, String description, int status, List<String> appointments = null)
        {
            Id = id;
            BoardUserId = boardUserId;
            MeetingTypeId = meetingTypeId;
            Subject = subject;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Description = description;
            Status = status;
            Appointments = appointments;
        }
    }
}
