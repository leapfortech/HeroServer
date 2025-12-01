using System;
using System.Collections.Generic;

namespace HpbServer
{
    public class PuzzleFull : PostFull
    {
        public long Id { get; set; }
        public long PuzzleTypeId { get; set; }
        public long PuzzleSubtypeId { get; set; }
        public String Question { get; set; }
        public String Hint { get; set; }
        public int Difficulty { get; set; }
        public int Points { get; set; }
        public int PlayCount { get; set; }
        public int Status { get; set; }

        public List<PuzzleAnswerFull> PuzzleAnswerFulls { get; set; }
        public List<PuzzleCommentFull> PuzzleCommentFulls { get; set; }


        public PuzzleFull(long id, long postId, long appUserId, String appUserAlias,
                          long postTypeId, long postSubtypeId,
                          long postOriginCountryId, long postOriginStateId,
                          String title, String summary, String description,
                          int imageCount, int likesCount, DateTime publicationDateTime,
                          int postStatus,
                          long puzzleTypeId, long puzzleSubtypeId,
                          String question, String hint,
                          int difficulty, int points, int playCount,
                          int status,
                          List<PuzzleAnswerFull> puzzleAnswerFulls,
                          List<PuzzleCommentFull> puzzleCommentFulls)
            : base(postId, appUserId, appUserAlias, postTypeId, postSubtypeId,
                   postOriginCountryId, postOriginStateId, title, summary, description,
                   imageCount, likesCount, publicationDateTime, postStatus)
        {
            Id = id;
            PuzzleTypeId = puzzleTypeId;
            PuzzleSubtypeId = puzzleSubtypeId;
            Question = question;
            Hint = hint;
            Difficulty = difficulty;
            Points = points;
            PlayCount = playCount;
            Status = status;

            PuzzleAnswerFulls = puzzleAnswerFulls ?? new List<PuzzleAnswerFull>();
            PuzzleCommentFulls = puzzleCommentFulls ?? new List<PuzzleCommentFull>();
        }
    }
}
