using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterPuzzleRequest : RegisterPostRequest
    {
        public Puzzle Puzzle { get; set; }
        public List<PuzzleAnswer> PuzzleAnswers { get; set; }
        public List<Comment> PuzzleComments { get; set; }

        public RegisterPuzzleRequest()
        {
        }

        public RegisterPuzzleRequest(Puzzle puzzle, List<PuzzleAnswer> puzzleAnswers, List<Comment> puzzleComments)
        {
            Puzzle = puzzle;
            PuzzleAnswers = puzzleAnswers;
            PuzzleComments = puzzleComments;
        }
    }
}
