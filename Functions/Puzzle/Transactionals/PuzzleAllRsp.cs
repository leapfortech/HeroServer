using System.Collections.Generic;

namespace HeroServer
{
    public class PuzzleAllRsp
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<PuzzleInfo> PuzzleInfos { get; set; }

        public PuzzleAllRsp()
        {
        }

        public PuzzleAllRsp(int page, int totalPages, List<PuzzleInfo> puzzleInfos)
        {
            Page = page;
            TotalPages = totalPages;
            PuzzleInfos = puzzleInfos;
        }
    }
}