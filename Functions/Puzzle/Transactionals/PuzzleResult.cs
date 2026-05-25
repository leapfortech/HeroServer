using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleResult
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public long PuzzleId { get; set; }
        public int TotalPoints { get; set; }
        public int TotalWinPoints { get; set; }
        public DateTime LastPlayDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public PuzzleResult()
        { 
        }

        public PuzzleResult(long id, long playerId, long puzzleId, int totalPoints, int totalWinPoints,
                            DateTime lastPlayDateTime, DateTime createDateTime, DateTime updateDateTime)
        {
            Id = id;
            PlayerId = playerId;
            PuzzleId = puzzleId;
            TotalPoints = totalPoints;
            TotalWinPoints = totalWinPoints;
            LastPlayDateTime = lastPlayDateTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
        }
    }
}
