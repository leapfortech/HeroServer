using System;

namespace HeroServer
{
    public class OfacAlias
    {
#pragma warning disable IDE1006 // Naming Styles
        public String id { get; set; }
        public String name { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public String comment { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacAlias()
        {
        }

        public OfacAlias(String id, String name, String firstName, String lastName, String comment)
        {
            this.id = id;
            this.name = name;
            this.firstName = firstName;
            this.lastName = lastName;
            this.comment = comment;
        }
    }
}
