
namespace HeroServer
{
    public class PuzzleAllByDifficultyReq
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long PuzzleGameId { get; set; }
        public int Difficulty { get; set; }
        public int Status { get; set; }

        public PuzzleAllByDifficultyReq()
        {
        }

        public PuzzleAllByDifficultyReq(int page, int pageSize, long puzzleGameId, int difficulty, int status)
        {
            Page = page;
            PageSize = pageSize;
            PuzzleGameId = puzzleGameId;
            Difficulty = difficulty;
            Status = status;
        }
    }
}