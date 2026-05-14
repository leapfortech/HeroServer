using System;

namespace HeroServer
{
    public class Faq
    {
        public long Id { get; set; }
        public long BoardUserId { get; set; }
        public long FaqTypeId { get; set; }
        public String Question { get; set; }
        public String Answer { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Faq()
        { 
        }

        public Faq(long id, long boardUserId, long faqTypeId, String question, String answer, DateTime createDateTime,
                   DateTime updateDateTime, int status)
        {
            Id = id;
            BoardUserId = boardUserId;
            FaqTypeId = faqTypeId;
            Question = question;
            Answer = answer;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
