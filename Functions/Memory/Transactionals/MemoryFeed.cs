using System;

namespace HeroServer
{
    public class MemoryFeed
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String TitleImage { get; set; }
        public String Title { get; set; }
        public String Country { get; set; }
        public String State { get; set; }
        public DateTime? DateTime { get; set; }
        public String MemoryType { get; set; }
        public int[] ReactionCounts { get; set; }
        public long ReactionPhraseId { get; set; }
        public int CommentCount { get; set; }
        public DateTime PublicationDateTime { get; set; }

        public MemoryFeed()
        {

        }

        public MemoryFeed(long id, long postId, String titleImage, String title, String country, String state, DateTime? dateTime,
                          String memoryType, int[] reactionCounts, long reactionPhraseId, int commentCount, DateTime publicationDateTime)
        {
            Id = id;
            PostId = postId;
            TitleImage = titleImage;
            Title = title;
            Country = country;
            State = state;
            DateTime = dateTime;
            MemoryType = memoryType;
            ReactionCounts = reactionCounts;
            ReactionPhraseId = reactionPhraseId;
            CommentCount = commentCount;
            PublicationDateTime = publicationDateTime;
        }
    }
}
