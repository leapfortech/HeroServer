using System.Collections.Generic;

namespace HeroServer
{
    public class TaleDataFull
    {
        public List<TaleFull> TaleFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public TaleDataFull()
        {
        }

        public TaleDataFull(List<TaleFull> taleFulls,
                            List<ContactFull> contactFulls,
                            List<LinkFull> linkFulls,
                            List<CommentFull> commentFulls)
        {
            TaleFulls = taleFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
