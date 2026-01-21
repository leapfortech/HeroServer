using System;

namespace HeroServer
{
    public class PuzzleAnswerFull
    {
        public long Id { get; set; }
        public String Description { get; set; }
        public int IsCorrect { get; set; }
        public int Status { get; set; }

        public PuzzleAnswerFull()
        {
        }

        public PuzzleAnswerFull(long id, String description, int isCorrect, int status)
        {
            Id = id;
            Description = description;
            IsCorrect = isCorrect;
            Status = status;
        }
    }
}
