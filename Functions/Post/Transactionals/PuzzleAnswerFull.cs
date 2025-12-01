using System;

namespace HpbServer
{
    public class PuzzleAnswerFull
    {
        public long Id { get; set; }
        public String Description { get; set; }
        public int IsCorrect { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public PuzzleAnswerFull()
        {
        }

        public PuzzleAnswerFull(long id, String description, int isCorrect, DateTime updateDateTime)
        {
            Id = id;
            Description = description;
            IsCorrect = isCorrect;
            UpdateDateTime = updateDateTime;
        }
    }
}
