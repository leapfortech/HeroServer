
namespace HeroServer
{
    public class PuzzleResultSummary
    {
        public long PuzzleGameId { get; set; }
        public int TotalPoints { get; set; }
        public int TotalMedals { get; set; }
        public int TotalCups { get; set; }

        public PuzzleResultSummary()
        {
        }

        public PuzzleResultSummary(long puzzleGameId, int totalPoints, int totalMedals, int totalCups)
        {
            PuzzleGameId = puzzleGameId;
            TotalPoints = totalPoints;
            TotalMedals = totalMedals;
            TotalCups = totalCups;
        }
    }
}
