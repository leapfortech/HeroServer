using System;

namespace HeroServer
{
    public class UserCredential
    {
        public String Username { get; set; }
        public String Password { get; set; }
        

        public UserCredential()
        {
        }
        public UserCredential(String username, String password)
        {
            Username = username;
            Password = password;
        }
    }
}
