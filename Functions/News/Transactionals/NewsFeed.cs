using System;

namespace HeroServer
{
    public class NewsFeed
    {
        public String Title { get; set; }
        public String Description { get; set; }
        public DateTime? DateTime { get; set; }
        public String Source { get; set; }
        public int[] ReactionCounts { get; set; }
        public long ReactionPhraseId { get; set; }
        public int CommentCount { get; set; }
        public String Alias { get; set; }

        public NewsFeed()
        {

        }

        public NewsFeed(String title, String description, DateTime? dateTime, String source, int[] reactionCounts, long reactionPhraseId, int commentCount, String alias)
        {
            Title = title;
            Description = description;
            DateTime = dateTime;
            Source = source;
            ReactionCounts = reactionCounts;
            ReactionPhraseId = reactionPhraseId;
            CommentCount = commentCount;
            Alias = alias;
        }
    }
}
