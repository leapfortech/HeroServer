using System;

namespace HeroServer
{
    public class Treatment
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String Ingredients { get; set; }
        public String Preparation { get; set; }
        public String Usage { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Treatment() { }

        public Treatment(long id, long postId, String ingredients, String preparation,
                         String usage, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            Ingredients = ingredients;
            Preparation = preparation;
            Usage = usage;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
