using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class GenValues(String tableName, List<String> values)
    {
        public String TableName { get; set; } = tableName;
        public List<String> Values { get; set; } = values;

        public String this[int idx]
        {
            get { return Values[idx]; }
        }
    }
}
