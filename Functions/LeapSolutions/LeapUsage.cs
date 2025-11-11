using System;

namespace HeroServer
{
    public class LeapUsage
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int ProductId { get; set; }
        public int ResponseCode { get; set; }
        public String ResponseMessage { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public LeapUsage()
        {
        }

        public LeapUsage(int id, int appUserId, int productId, int responseCode, String responseMessage, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            ProductId = productId;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public LeapUsage(int appUserId, int productId, int responseCode, String responseMessage)
        {
            Id = -1;
            AppUserId = appUserId;
            ProductId = productId;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = 1;
        }
    }
}
