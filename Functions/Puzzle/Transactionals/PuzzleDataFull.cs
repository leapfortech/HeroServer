using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleDataFull
    {
        public List<PuzzleFull> PuzzleFulls { get; set; }
        public List<PuzzleAnswerFull> PuzzleAnswerFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public PuzzleDataFull()
        {
        }

        public PuzzleDataFull(List<PuzzleFull> puzzleFulls,
                              List<PuzzleAnswerFull> puzzleAnswerFulls,
                              List<ContactFull> contactFulls,
                              List<LinkFull> linkFulls,
                              List<CommentFull> commentFulls)
        {
            PuzzleFulls = puzzleFulls;
            PuzzleAnswerFulls = puzzleAnswerFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
