using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class News
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Link { get; set; }
        public String Content { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public News()
        {
        }

        public News(int id, String title, String description, String link, String content, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            Title = title;
            Description = description;
            Link = link;
            Content = content;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
