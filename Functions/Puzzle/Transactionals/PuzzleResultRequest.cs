
namespace HeroServer
{
    public class PuzzleResultRequest
    {
        public long PlayerId { get; set; }
        public long PuzzleId { get; set; }
        public long PuzzleAnswerId { get; set; }
        public int Time { get; set; }

        public PuzzleResultRequest()
        { 
        }

        public PuzzleResultRequest(long playerId, long puzzleId, long puzzleAnswerId, int time)
        {
            PlayerId = playerId;
            PuzzleId = puzzleId;
            PuzzleAnswerId = puzzleAnswerId;
            Time = time;
        }
    }
}
