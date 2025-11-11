using System;

namespace HeroServer
{
    public class Signature
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public String Strokes { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int Status { get; set; }


        public Signature()
        {
        }

        public Signature(int id, int appUserId, String strokes, DateTime createDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            Strokes = strokes;
            CreateDateTime = createDateTime;
            Status = status;
        }
    }
}
