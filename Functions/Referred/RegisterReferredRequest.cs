namespace HeroServer
{
    public class RegisterReferredRequest
    {
        public long AppUserId { get; set; }
        public Identity Identity { get; set; }

        public RegisterReferredRequest()
        {
        }

        public RegisterReferredRequest(long appUserId, Identity identity)
        {
            AppUserId = appUserId;
            Identity = Identity;
        }
    }
}
