using System;

namespace HeroServer
{
    public class AppParam
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Value { get; set; }


        public AppParam()
        {
        }

        public AppParam(int id, String name, String value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }
}
