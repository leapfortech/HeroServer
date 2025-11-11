using System;

namespace HeroServer
{
    public class FirebaseRoles
    {
        public String AuthUserId { get; set; }
        public String[] Roles { get; set; }

        public FirebaseRoles()
        {
        }

        public FirebaseRoles(String authUserId, String[] roles)
        {
            AuthUserId = authUserId;
            Roles = roles;
        }
    }
}
