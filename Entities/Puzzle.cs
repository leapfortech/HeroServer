using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class Puzzle
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long PuzzleTypeId { get; set; }
        public long PuzzleSubtypeId { get; set; }
        public String Question { get; set; }
        public String Hint { get; set; }
        public int Difficulty { get; set; }
        public int Points { get; set; }
        public int PlayCount { get; set; }
        public int CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Puzzle() { }

        public Puzzle(long id, long postId, long puzzleTypeId, long puzzleSubtypeId, String question, String hint,
                      int difficulty, int points, int playCount, int createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            PuzzleTypeId = puzzleTypeId;
            PuzzleSubtypeId = puzzleSubtypeId;
            Question = question;
            Hint = hint;
            Difficulty = difficulty;
            Points = points;
            PlayCount = playCount;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
