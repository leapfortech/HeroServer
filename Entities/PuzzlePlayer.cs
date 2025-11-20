using System;

namespace HeroServer
{
    public class PuzzlePlayer
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public long PuzzleId { get; set; }
        public int IsGuessed { get; set; }
        public DateTime? AttemptDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }

        public PuzzlePlayer() { }

        public PuzzlePlayer(long id, long playerId, long puzzleId, int isGuessed, DateTime? attemptDateTime,
                            DateTime createDateTime)
        {
            Id = id;
            PlayerId = playerId;
            PuzzleId = puzzleId;
            IsGuessed = isGuessed;
            AttemptDateTime = attemptDateTime;
            CreateDateTime = createDateTime;
        }
    }
}
