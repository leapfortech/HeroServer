using System;

namespace HeroServer
{
    public class RadioLanguage
    {
        public long Id { get; set; }
        public long RadioId { get; set; }
        public long LanguageId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public RadioLanguage()
        {
        }

        public RadioLanguage(long id, long radioId, long languageId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            RadioId = radioId;
            LanguageId = languageId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public RadioLanguage(RadioLanguageFull radioLanguageFull)
        {
            Id = -1;
            RadioId = radioLanguageFull.Id;
            LanguageId = radioLanguageFull.LanguageId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = radioLanguageFull.Status;
        }
    }
}
