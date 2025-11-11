using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class Appointment
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public int AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Appointment()
        {
        }

        public Appointment(int id, int meetingId, int appUserId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            MeetingId = meetingId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
