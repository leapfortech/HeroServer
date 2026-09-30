using System;

namespace HeroServer
{
    public class PuzzleAnswerFull
    {
        public long Id { get; set; }
        public long PuzzleId { get; set; }
        public String Description { get; set; }
        public int IsCorrect { get; set; }
        public int Status { get; set; }

        public PuzzleAnswerFull()
        {
        }

        public PuzzleAnswerFull(long id, long puzzleId, String description, int isCorrect, int status)
        {
            Id = id;
            PuzzleId = puzzleId;
            Description = description;
            IsCorrect = isCorrect;
            Status = status;
        }
    }
}
