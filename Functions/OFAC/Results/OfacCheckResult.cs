
namespace HeroServer
{
    public class OfacCheckResult : LeapResult
    {
        public OfacPerson[] Persons { get; set; }

        public OfacCheckResult()
        {
            Persons = null;
        }

        public OfacCheckResult(OfacScreenResponse ofacScreenResponse)
        {
            Persons = new OfacPerson[ofacScreenResponse.results[0].matchCount];

            for (int i = 0; i < Persons.Length; i++)
                Persons[i] = new OfacPerson(ofacScreenResponse.results[0].matches[i].sanction);
        }
    }
}