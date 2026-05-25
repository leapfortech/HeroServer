using System;

namespace HeroServer
{
    public class Puzzle
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long PuzzleSubtypeId { get; set; }
        public long CountryId { get; set; }
        public String Question { get; set; }
        public String Hint { get; set; }
        public int Difficulty { get; set; }
        public int Delay { get; set; }
        public int Points { get; set; }
        public int PlayCount { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Puzzle() { }

        public Puzzle(long id, long postId, long puzzleSubtypeId, long countryId, String question, String hint,
                      int difficulty, int delay, int points, int playCount, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            PuzzleSubtypeId = puzzleSubtypeId;
            CountryId = countryId;
            Question = question;
            Hint = hint;
            Difficulty = difficulty;
            Delay = delay;
            Points = points;
            PlayCount = playCount;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
