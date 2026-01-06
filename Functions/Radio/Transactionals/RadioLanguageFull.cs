using System;

namespace HeroServer
{
    public class RadioLanguageFull
    {
        public long Id { get; set; }
        public long LanguageId { get; set; }
        public int Status { get; set; }

        public RadioLanguageFull()
        {
        }

        public RadioLanguageFull(long id, long languageId, int status)
        {
            Id = id;
            LanguageId = languageId;
            Status = status;
        }
    }
}
