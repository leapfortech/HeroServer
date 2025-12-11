using System.Collections.Generic;

namespace HeroServer
{
    public class RadioDataFull
    {
        public List<RadioFull> RadioFulls { get; set; }
        public List<RadioTypeFull> RadioTypeFulls { get; set; }
        public List<RadioLanguageFull> RadioLanguageFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public RadioDataFull()
        {
        }

        public RadioDataFull(List<RadioFull> radioFulls,
                              List<RadioTypeFull> radioTypeFulls,
                              List<RadioLanguageFull> radioLanguageFulls,
                              List<ContactFull> contactFulls,
                              List<LinkFull> linkFulls,
                              List<CommentFull> commentFulls)
        {
            RadioFulls = radioFulls;
            RadioTypeFulls = radioTypeFulls;
            RadioLanguageFulls = radioLanguageFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
