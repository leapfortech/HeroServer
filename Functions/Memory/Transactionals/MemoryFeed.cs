using System;

namespace HeroServer
{
    public class MemoryFeed
    {
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

        public MemoryFeed(String titleImage, String title, String country, String state, DateTime? dateTime, int[] reactionCounts, long reactionPhraseId)
        {
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
