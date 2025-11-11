using System.Threading.Tasks;

namespace HeroServer
{
    public static class OfacFunctions
    {
        public static async Task<LeapResponse> Check(OfacRequest ofacRequest)
        {
            return await LeapServerHelper.Post<OfacRequest, LeapResponse>("ofac/Check", ofacRequest);
        }

        public static async Task<LeapResponse> Find(OfacRequest ofacRequest)
        {
            return await LeapServerHelper.Post<OfacRequest, LeapResponse>("ofac/Find", ofacRequest);
        }
    }
}