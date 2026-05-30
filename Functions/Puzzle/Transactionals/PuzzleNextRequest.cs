
namespace HeroServer
{
    public class PuzzleNextRequest
    {
        public long PlayerId { get; set; }
        public long PuzzleGameId { get; set; }
        public long CountryId { get; set; }
        public int Difficulty { get; set; }

        public PuzzleNextRequest()
        { 
        }

        public PuzzleNextRequest(long playerId, long puzzleGameId, long countryId, int difficulty)
        {
            PlayerId = playerId;
            PuzzleGameId = puzzleGameId;
            CountryId = countryId;
            Difficulty = difficulty;
        }
    }
}
