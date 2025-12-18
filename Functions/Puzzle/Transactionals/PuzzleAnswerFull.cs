using System;

namespace HeroServer
{
    public class PuzzleAnswerFull
    {
        public long Id { get; set; }
        public String Description { get; set; }
        public int IsCorrect { get; set; }

        public PuzzleAnswerFull()
        {
        }

        public PuzzleAnswerFull(long id, String description, int isCorrect)
        {
            Id = id;
            Description = description;
            IsCorrect = isCorrect;
        }
    }
}
