using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleFull : PostFull
    {
        public long Id { get; set; }
        public long PuzzleSubtypeId { get; set; }
        public long CountryId { get; set; }
        public String Question { get; set; }
        public String Hint { get; set; }
        public int Difficulty { get; set; }
        public int Points { get; set; }
        public int PlayCount { get; set; }
        public int Status { get; set; }

        public List<PuzzleAnswerFull> PuzzleAnswerFulls { get; set; }

        public List<String> Images { get; set; }


        public PuzzleFull(long id, long postId, long appUserId, String appUserAlias,
                          long postTypeId,
                          long postCountryId, long postStateId,
                          String title, String titleImage, String summary, String description,
                          int imageCount, int likeCount, DateTime publicationDateTime,
                          int postStatus,
                          ContactFull contactFull,
                          List<LinkFull> linkFulls,
                          List<CommentFull> commentFulls,
                          long puzzleSubtypeId, long countryId,
                          String question, String hint,
                          int difficulty, int points, int playCount,
                          int status,
                          List<PuzzleAnswerFull> puzzleAnswerFulls,
                          List<String> images)
            : base(postId, appUserId, appUserAlias, postTypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatus,
                   contactFull, linkFulls, commentFulls)
        {
            Id = id;
            PuzzleSubtypeId = puzzleSubtypeId;
            CountryId = countryId;
            Question = question;
            Hint = hint;
            Difficulty = difficulty;
            Points = points;
            PlayCount = playCount;
            Status = status;

            PuzzleAnswerFulls = puzzleAnswerFulls ?? new List<PuzzleAnswerFull>();
            Images = images;
        }
    }
}
