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
        public int[] ReactionCounts { get; set; }
        public long ReactionPhraseId { get; set; }

        public MemoryFeed()
        {

        }

        public MemoryFeed(long id, long postId, String titleImage, String title, String country, String state, DateTime? dateTime, int[] reactionCounts, long reactionPhraseId)
        {
            Id = id;
            PostId = postId;
            TitleImage = titleImage;
            Title = title;
            Country = country;
            State = state;
            DateTime = dateTime;
            ReactionCounts = reactionCounts;
            ReactionPhraseId = reactionPhraseId;
        }
    }
}
