
namespace HeroServer
{
    public class PuzzleAllByDifficultyReq
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long PuzzleSubtypeId { get; set; }
        public int Difficulty { get; set; }
        public int Status { get; set; }

        public PuzzleAllByDifficultyReq()
        {
        }

        public PuzzleAllByDifficultyReq(int page, int pageSize, long puzzleSubtypeId, int difficulty, int status)
        {
            Page = page;
            PageSize = pageSize;
            PuzzleSubtypeId = puzzleSubtypeId;
            Difficulty = difficulty;
            Status = status;
        }
    }
}