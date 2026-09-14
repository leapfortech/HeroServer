using System.Collections.Generic;

namespace HeroServer
{
    public class MemoryDataFull
    {
        public List<MemoryFull> MemoryFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public MemoryDataFull()
        {
        }

        public MemoryDataFull(List<MemoryFull> memoryFulls,
                              List<ContactFull> contactFulls,
                              List<LinkFull> linkFulls,
                              List<CommentFull> commentFulls)
        {
            MemoryFulls = memoryFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
