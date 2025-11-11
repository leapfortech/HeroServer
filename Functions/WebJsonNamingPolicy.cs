using System;
using System.Text.Json;

namespace HeroServer
{
    public class WebJsonNamingPolicy : JsonNamingPolicy
    {
        public override String ConvertName(String name)
        {
            if (name.Length == 1)
                return name;
            return char.ToUpper(name[0]) + name[1..];
        }
    }
}
