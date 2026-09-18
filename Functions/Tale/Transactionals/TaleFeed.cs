using System;

namespace HeroServer
{
    public class TaleFeed
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String TitleImage { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public int[] ReactionCounts { get; set; }
        public long ReactionPhraseId { get; set; }
        public int CommentCount { get; set; }
        public String Alias { get; set; }
        public String InterestLocality { get; set; }
        public String CurrentLocality { get; set; }

        public TaleFeed()
        {

        }

        public TaleFeed(long id, long postId, String titleImage, String title, String description, int[] reactionCounts, long reactionPhraseId, int commentCount, String alias, String interestLocality, String currentLocality)
        {
            Id = id;
            PostId = postId;
            TitleImage = titleImage;
            Title = title;
            Description = description;
            ReactionCounts = reactionCounts;
            ReactionPhraseId = reactionPhraseId;
            CommentCount = commentCount;
            Alias = alias;
            InterestLocality = interestLocality;
            CurrentLocality = currentLocality;
        }
    }
}

