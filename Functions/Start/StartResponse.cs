using System;

namespace HeroServer
{
    public class StartResponse
    {
        public String Certificates { get; set; }
        public int Granted { get; set; }
        public String Message { get; set; }
        public String Link { get; set; }

        public StartResponse()
        {
        }

        public StartResponse(String certificates, int granted, String message = null, String link = null)
        {
            Certificates = certificates;
            Granted = granted;
            Message = message;
            Link = link;
        }

        public StartResponse(int granted, String message = null, String link = null)
        {
            Certificates = null;
            Granted = granted;
            Message = message;
            Link = link;
        }
    }
}
