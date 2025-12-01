using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterPuzzleRequest : RegisterPostRequest
    {
        public Puzzle Puzzle { get; set; }
        public List<PuzzleAnswer> PuzzleAnswers { get; set; }
        public List<PuzzleComment> PuzzleComments { get; set; }

        public RegisterPuzzleRequest()
        {
        }

        public RegisterPuzzleRequest(Puzzle puzzle, List<PuzzleAnswer> puzzleAnswers, List<PuzzleComment> puzzleComments)
        {
            Puzzle = puzzle;
            PuzzleAnswers = puzzleAnswers;
            PuzzleComments = puzzleComments;
        }
    }
}
