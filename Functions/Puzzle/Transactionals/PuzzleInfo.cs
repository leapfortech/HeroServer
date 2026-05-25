
using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleInfo
    {
        public Post Post { get; set; }
        public Puzzle Puzzle { get; set; }
        public List<PuzzleAnswer> PuzzleAnswers { get; set; }

        public PuzzleInfo()
        {
        }

        public PuzzleInfo(Post post, Puzzle puzzle, List<PuzzleAnswer> puzzleAnswers)
        {
            Post = post;
            Puzzle = puzzle;
            PuzzleAnswers = puzzleAnswers;
        }
    }
}