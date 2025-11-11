using System;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class GenValuesFunctions
    {
        // Get
        public static async Task<int> GetIdByCode(String table, String code)
        {
            return await new GenValuesDB().GetIdByCode(table, code);
        }

        public static async Task<String> GetCodeById(String table, int id)
        {
            return await new GenValuesDB().GetCodeById(table, id);
        }

        public static async Task<String> GetNameById(String table, int id)
        {
            return await new GenValuesDB().GetNameById(table, id);
        }

        public static async Task<String> GetNameByCode(String table, String code)
        {
            return await new GenValuesDB().GetNameByCode(table, code);
        }

        public static async Task<String> GetStringById(String table, int id, String fieldName)
        {
            return await new GenValuesDB().GetStringById(table, id, fieldName);
        }

        public static async Task<int> GetIntById(String table, int id, String fieldName)
        {
            return await new GenValuesDB().GetIntById(table, id, fieldName);
        }

        public static async Task<int> GetIdByField(String table, String fieldName, String fieldValue)
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
    }
}