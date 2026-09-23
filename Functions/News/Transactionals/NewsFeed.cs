using System;

namespace HeroServer
{
    public class NewsFeed
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String TitleImage { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public DateTime? DateTime { get; set; }
        public String NewsType { get; set; }
        public String Source { get; set; }
        public String Alias { get; set; }
        public int[] ReactionCounts { get; set; }
        public long ReactionPhraseId { get; set; }
        public int CommentCount { get; set; }
        public DateTime PublicationDateTime { get; set; }

        public NewsFeed()
        {

        }

        public NewsFeed(long id, long postId, String titleImage, String title, String description, DateTime? dateTime, String newsType, String source, String alias, int[] reactionCounts, long reactionPhraseId, int commentCount, DateTime publicationDateTime)
        {
            Id = id;
            PostId = postId;
            TitleImage = titleImage;
            Title = title;
            Description = description;
            DateTime = dateTime;
            NewsType = newsType;
            Source = source;
            ReactionCounts = reactionCounts;
            ReactionPhraseId = reactionPhraseId;
            CommentCount = commentCount;
            Alias = alias;
            PublicationDateTime = publicationDateTime;
        }
    }
}
