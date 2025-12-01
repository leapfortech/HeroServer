using System;

namespace HeroServer
{
    public class AliasResponse
    {
        public String Email { get; set; }

        public AliasResponse()
        {
        }

        public AliasResponse(String email)
        {
            Email = email;
        }
    }
}
