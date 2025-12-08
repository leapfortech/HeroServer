using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleDataFull
    {
        public List<PuzzleFull> PuzzleFulls { get; set; }
        public List<PuzzleAnswerFull> PuzzleAnswerFulls { get; set; }
        public List<PuzzleCommentFull> PuzzleCommentFulls { get; set; }

        public PuzzleDataFull()
        {
        }

        public PuzzleDataFull(List<PuzzleFull> puzzleFulls,
                              List<PuzzleAnswerFull> puzzleAnswerFulls,
                              List<PuzzleCommentFull> puzzleCommentFulls)
        {
            PuzzleFulls = puzzleFulls;
            PuzzleAnswerFulls = puzzleAnswerFulls;
            PuzzleCommentFulls = puzzleCommentFulls;
        }
    }
}
