using System;
using Newtonsoft.Json;

namespace HeroServer.Security
{
    public class FirebaseUserInfo
    {
        [JsonProperty(PropertyName = "identities")]
        public FirebaseIdentities Identities { get; set; }

        [JsonProperty(PropertyName = "sign_in_provider")]
        public String SignInProvider { get; set; }
    }

    public class FirebaseIdentities
    {
        [JsonProperty(PropertyName = "facebook.com")]
        public String[] FacebookDotCom { get; set; }

        [JsonProperty(PropertyName = "google.com")]
        public String[] GoogleDotCom { get; set; }

        [JsonProperty(PropertyName = "email")]
        public String[] Email { get; set; }
    }
}
