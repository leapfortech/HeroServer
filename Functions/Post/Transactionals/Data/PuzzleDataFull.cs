using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleDataFull
    {
        public List<PuzzleFull> PuzzleFulls { get; set; }
        public List<PuzzleAnswerFull> PuzzleAnswerFulls { get; set; }
        public List<CommentFull> PuzzleCommentFulls { get; set; }

        public PuzzleDataFull()
        {
        }

        public PuzzleDataFull(List<PuzzleFull> puzzleFulls,
                              List<PuzzleAnswerFull> puzzleAnswerFulls,
                              List<CommentFull> puzzleCommentFulls)
        {
            PuzzleFulls = puzzleFulls;
            PuzzleAnswerFulls = puzzleAnswerFulls;
            PuzzleCommentFulls = puzzleCommentFulls;
        }
    }
}
