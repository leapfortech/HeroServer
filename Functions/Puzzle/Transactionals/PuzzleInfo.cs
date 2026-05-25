
namespace HeroServer
{
    public class PuzzleInfo
    {
        public Puzzle Puzzle { get; set; }
        public PuzzleAnswer PuzzleAnswer { get; set; }

        public PuzzleInfo()
        {
        }

        public PuzzleInfo(Puzzle puzzle, PuzzleAnswer puzzleAnswer)
        {
            Puzzle = puzzle;
            PuzzleAnswer = puzzleAnswer;
        }
    }
}