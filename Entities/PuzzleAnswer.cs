using System;

namespace HeroServer
{
    public class PuzzleAnswer
    {
        public long Id { get; set; }
        public long PuzzleId { get; set; }
        public String Description { get; set; }
        public int IsCorrect { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public PuzzleAnswer() { }

        public PuzzleAnswer(long id, long puzzleId, String description, int isCorrect, DateTime createDateTime,
                            DateTime updateDateTime, int status)
        {
            Id = id;
            PuzzleId = puzzleId;
            Description = description;
            IsCorrect = isCorrect;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
