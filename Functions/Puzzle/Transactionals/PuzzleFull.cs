using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleFull : PostFull
    {
        public long Id { get; set; }
        public long PuzzleGameId { get; set; }
        public long CountryId { get; set; }
        public String Question { get; set; }
        public String Hint { get; set; }
        public int Difficulty { get; set; }
        public int Delay { get; set; }
        public int Points { get; set; }
        public int PlayCount { get; set; }
        public int Status { get; set; }

        public List<PuzzleAnswerFull> PuzzleAnswerFulls { get; set; }

        public List<String> Images { get; set; }


        public PuzzleFull(long id, long postId, long appUserId, String appUserAlias,
                          long postTypeId,
                          long postCountryId, long postStateId,
                          String title, String titleImage, String summary, String description,
                          int imageCount, int favorite, int like, int likeCount, long reactionPhraseId,
                          DateTime publicationDateTime, int postStatus,
                          ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls,
                          long puzzleGameId, long countryId,
                          String question, String hint,
                          int difficulty, int delay, int points, int playCount,
                          int status,
                          List<PuzzleAnswerFull> puzzleAnswerFulls,
                          List<String> images)
            : base(postId, appUserId, appUserAlias, postTypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, favorite, like, likeCount, reactionPhraseId, publicationDateTime, postStatus,
                   contactFull, linkFulls, commentFulls)
        {
            Id = id;
            PuzzleGameId = puzzleGameId;
            CountryId = countryId;
            Question = question;
            Hint = hint;
            Difficulty = difficulty;
            Delay = delay;
            Points = points;
            PlayCount = playCount;
            Status = status;

            PuzzleAnswerFulls = puzzleAnswerFulls ?? new List<PuzzleAnswerFull>();
            Images = images;
        }
    }
}
