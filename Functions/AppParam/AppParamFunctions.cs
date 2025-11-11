using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class AppParamFunctions
    {
        // Get
        public static async Task<IEnumerable<AppParam>> GetAll()
        {
            return await new AppParamDB().GetAll();
        }

        public static async Task<String> GetValue(String key)
        {
            return await new AppParamDB().GetValue(key);
        }
    }
}