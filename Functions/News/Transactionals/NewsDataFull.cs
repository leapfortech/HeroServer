using System.Collections.Generic;

namespace HeroServer
{
    public class NewsDataFull
    {
        public List<NewsFull> NewsFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public NewsDataFull()
        {
        }

        public NewsDataFull(List<NewsFull> newsFulls,
                            List<ContactFull> contactFulls,
                            List<LinkFull> linkFulls,
                            List<CommentFull> commentFulls)
        {
            NewsFulls = newsFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
