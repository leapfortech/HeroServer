using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class ProjectImages
    {
        public int Id { get; set; }
        public List<String> Images { get; set; }

        public ProjectImages()
        {
        }

        public ProjectImages(int id, List<String> images)
        {
            Id = id;
            Images = images;
        }
    }
}
