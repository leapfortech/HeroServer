
namespace HeroServer
{
    public class PuzzleResultResponse
    {
        public int Correct { get; set; }
        public int Points { get; set; }
        public int NewMedals { get; set; }
        public int NewCups { get; set; }

        public PuzzleFull PuzzleFull { get; set; }

        public PuzzleResultResponse()
        { 
        }

        public PuzzleResultResponse(int correct, int points, int newMedals, int newCups, PuzzleFull puzzleFull)
        {
            Correct = correct;
            Points = points;
            NewMedals = newMedals;
            NewCups = newCups;
            PuzzleFull = puzzleFull;
        }
    }
}
