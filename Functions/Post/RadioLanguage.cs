using System;

namespace HeroServer
{
    public class RadioLanguage
    {
        public long Id { get; set; }
        public long RadioId { get; set; }
        public long LanguageTypeId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public RadioLanguage() { }

        public RadioLanguage(long id, long radioId, long languageTypeId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            RadioId = radioId;
            LanguageTypeId = languageTypeId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
