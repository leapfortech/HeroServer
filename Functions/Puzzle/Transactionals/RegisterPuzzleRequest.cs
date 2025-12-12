using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterPuzzleRequest : RegisterPostRequest
    {
        public Puzzle Puzzle { get; set; }
        public List<PuzzleAnswer> PuzzleAnswers { get; set; }

        public RegisterPuzzleRequest()
        {
        }

        public RegisterPuzzleRequest(Puzzle puzzle, List<PuzzleAnswer> puzzleAnswers)
        {
            Puzzle = puzzle;
            PuzzleAnswers = puzzleAnswers;
        }
    }
}
