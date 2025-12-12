using System.Collections.Generic;

namespace HeroServer
{
    public class HappeningDataFull
    {
        public List<HappeningFull> HappeningFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public HappeningDataFull()
        {
        }

        public HappeningDataFull(List<HappeningFull> happeningFulls,
                                 List<ContactFull> contactFulls,
                                 List<LinkFull> linkFulls,
                                 List<CommentFull> commentFulls)
        {
            HappeningFulls = happeningFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
