using System;

namespace HeroServer
{
    public class Player
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public int Rank { get; set; }
        public int PuzzleCount { get; set; }
        public int TotalPoints { get; set; }
        public DateTime LastPlayDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public Player() { }

        public Player(long id, long appUserId, int rank, int puzzleCount, int totalPoints, DateTime lastPlayDateTime,
                      DateTime createDateTime, DateTime updateDateTime)
        {
            Id = id;
            AppUserId = appUserId;
            Rank = rank;
            PuzzleCount = puzzleCount;
            TotalPoints = totalPoints;
            LastPlayDateTime = lastPlayDateTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
        }
    }
}
