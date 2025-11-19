using System;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class GenValuesFunctions
    {
        // Get
        public static async Task<long> GetIdByCode(String table, String code)
        {
            return await new GenValuesDB().GetIdByCode(table, code);
        }

        public static async Task<String> GetCodeById(String table, long id)
        {
            return await new GenValuesDB().GetCodeById(table, id);
        }

        public static async Task<String> GetNameById(String table, long id)
        {
            return await new GenValuesDB().GetNameById(table, id);
        }

        public static async Task<String> GetNameByCode(String table, String code)
        {
            return await new GenValuesDB().GetNameByCode(table, code);
        }

        public static async Task<String> GetStringById(String table, long id, String fieldName)
        {
            return await new GenValuesDB().GetStringById(table, id, fieldName);
        }

        public static async Task<int> GetIntById(String table, long id, String fieldName)
        {
            return await new GenValuesDB().GetIntById(table, id, fieldName);
        }

        public static async Task<long> GetLongById(String table, long id, String fieldName)
        {
            return await new GenValuesDB().GetLongById(table, id, fieldName);
        }

        public static async Task<long> GetIdByField(String table, String fieldName, String fieldValue)
        {
            return await new GenValuesDB().GetIdByField(table, fieldName, fieldValue);
        }

        public static async Task<String> GetStringByIntFields(String table, String[] keyNames, int[] keyValues, String fieldName)
        {
            return await new GenValuesDB().GetStringByIntFields(table, keyNames, keyValues, fieldName);
        }

        public static async Task<int> GetIntByIntFields(String table, String[] keyNames, int[] keyValues, String fieldName)
        {
            return await new GenValuesDB().GetIntByIntFields(table, keyNames, keyValues, fieldName);
        }

        public static async Task<long> GetLongByIntFields(String table, String[] keyNames, int[] keyValues, String fieldName)
        {
            return await new GenValuesDB().GetLongByIntFields(table, keyNames, keyValues, fieldName);
        }
    }
}