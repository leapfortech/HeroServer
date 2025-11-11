using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class SystemParam
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Value { get; set; }


        public SystemParam()
        {
        }

        public SystemParam(int id, String name, String value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }
}
