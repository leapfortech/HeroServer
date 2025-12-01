using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterPostRequest
    {
        public Post Post { get; set; }
        public List<String> Images { get; set; }

        public RegisterPostRequest()
        {
        }

        public RegisterPostRequest(Post post, List<String> images)
        {
            Post = post;
            Images = images;
        }
    }
}
