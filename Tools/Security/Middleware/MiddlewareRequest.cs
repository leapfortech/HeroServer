using System;

namespace HeroServer.Middleware
{
    public class MiddlewareRequest
    {
        public String Verb { get; set; }
        public String Uri { get; set; }
        public MiddlewareHeader[] Headers { get; set; }
        public String Body { get; set; }
    }

    public class MiddlewareHeader
    {
        public String K { get; set; }
        public String V { get; set; }
    }
}