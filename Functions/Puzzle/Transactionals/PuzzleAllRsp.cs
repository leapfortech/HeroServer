using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleAllRsp
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<PuzzleFull> PuzzleFulls { get; set; }

        public PuzzleAllRsp()
        {
        }

        public PuzzleAllRsp(int page, int totalPages, List<PuzzleFull> puzzleFulls)
        {
            Page = page;
            TotalPages = totalPages;
            PuzzleFulls = puzzleFulls;
        }
    }
}