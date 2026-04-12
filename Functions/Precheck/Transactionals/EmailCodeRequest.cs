using System;

namespace HeroServer
{
    public class EmailCodeRequest
    {
        public String Email { get; set; }
        public String Code { get; set; }


        public EmailCodeRequest()
        {
        }

        public EmailCodeRequest(String email, String code)
        {
            Email = email;
            Code = code;
        }
    }
}
