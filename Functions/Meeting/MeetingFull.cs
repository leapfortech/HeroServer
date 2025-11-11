using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class MeetingFull
    {
        public int Id { get; set; }
        public String MeetingType { get; set; }
        public String Subject { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public String Description { get; set; }
        public int Status { get; set; }

        public MeetingFull()
        {
        }

        public MeetingFull(int id, String meetingType, String subject, DateTime startDateTime, DateTime endDateTime, String description, int status)
        {
            Id = id;
            MeetingType = meetingType;
            Subject = subject;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Description = description;
            Status = status;
        }
    }
}
