using System;

namespace HeroServer
{
    public class RenapIdentityInfo
    {
        public RenapIdentity RenapIdentity { get; set; }
        public String Face { get; set; }

        public RenapIdentityInfo()
        {
        }

        public RenapIdentityInfo(RenapIdentity renapIdentity, String face)
        {
            RenapIdentity = renapIdentity;
            Face = face;
        }
    }
}
