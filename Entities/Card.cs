using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class Card
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public String CSToken { get; set; }
        public int TypeId { get; set; }
        public String Number { get; set; }
        public int Digits { get; set; }
        public DateTime ExpirationDate { get; set; }
        public String Holder { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Card()
        {
        }

        public Card(int id, int appUserId, String csToken, int typeId, String number, int digits, DateTime expirationDate, String holder,
                    DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            CSToken = csToken;
            TypeId = typeId;
            Number = number;
            Digits = digits;
            ExpirationDate = expirationDate;
            Holder = holder;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public Card(int id, int appUserId, String csToken, int typeId, String number, int digits, DateTime expirationDate, String holder, int status)
        {
            Id = id;
            AppUserId = appUserId;
            CSToken = csToken;
            TypeId = typeId;
            Number = number;
            Digits = digits;
            ExpirationDate = expirationDate;
            Holder = holder;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = status;
        }
    }
}
