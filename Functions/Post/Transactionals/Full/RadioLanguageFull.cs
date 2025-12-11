using System;

namespace HeroServer
{
    public class RadioLanguageFull
    {
        public long Id { get; set; }
        public long LanguageTypeId { get; set; }
        public int Status { get; set; }

        public RadioLanguageFull()
        {
        }

        public RadioLanguageFull(long id, long languageTypeId, int status)
        {
            Id = id;
            LanguageTypeId = languageTypeId;
            Status = status;
        }
    }
}
