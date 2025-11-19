using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class GenValuesDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);

        // SELECT
        public async Task<GenValues> Get(GenValuesParams valuesParams)
        {
            String strCmd = GetCommand(valuesParams);

            SqlCommand command = new SqlCommand(strCmd, conn);

            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    return await GetGenValues(reader, valuesParams);
                }
            }
        }

        public async Task<GenValuesList> Get(GenValuesParams[] valuesParams)
        {
            String strCmd = "";

            for (int i = 0; i < valuesParams.Length; i++)
                strCmd += GetCommand(valuesParams[i]);

            SqlCommand command = new SqlCommand(strCmd, conn);

            GenValuesList genValuesList = new GenValuesList(valuesParams.Length);
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    genValuesList.Add(await GetGenValues(reader, valuesParams[0]));
                    for (int i = 1; i < valuesParams.Length; i++)
                    {
                        if (!reader.NextResult())
                            break;
                        genValuesList.Add(await GetGenValues(reader, valuesParams[i]));
                    }
                }
            }

            return genValuesList;
        }

        private static String GetCommand(GenValuesParams valuesParams)
        {
            if (valuesParams.TableName[0] != 'K')
                return "";

            String strCmd = "SELECT Id, " + valuesParams.Columns[0];
            for (int i = 1; i < valuesParams.Columns.Length; i++)
                strCmd += ", " + valuesParams.Columns[i];
            for (int i = 0; i < valuesParams.FilterColumns.Length; i++)
                strCmd += ", " + valuesParams.FilterColumns[i];

            strCmd += " FROM [" + valuesParams.TableName + "]";

            if (!String.IsNullOrEmpty(valuesParams.Status))
            {
                if (valuesParams.FilterStatus >= 0)
                    strCmd += " WHERE " + valuesParams.Status + " = " + valuesParams.FilterStatus;
                else
                    strCmd += " WHERE ([" + valuesParams.Status + "] & " + (-valuesParams.FilterStatus).ToString() + ") = " + (-valuesParams.FilterStatus).ToString();
            }

            strCmd += "; ";

            return strCmd;
        }

        private static async Task<GenValues> GetGenValues(SqlDataReader reader, GenValuesParams valuesParams)
        {
            List<String> results = [];
            while (await reader.ReadAsync())
            {
                String result = reader["Id"].ToString();
                result += "|" + reader[valuesParams.Columns[0]].ToString();

                for (int i = 1; i < valuesParams.Columns.Length; i++)
                    result += "^" + reader[valuesParams.Columns[i]].ToString();
                if (valuesParams.FilterColumns.Length > 0)
                {
                    result += "|" + reader[valuesParams.FilterColumns[0]].ToString();
                    for (int i = 1; i < valuesParams.FilterColumns.Length; i++)
                        result += "^" + reader[valuesParams.FilterColumns[i]].ToString();
                }

                results.Add(result);
            }

            return new GenValues(valuesParams.TableName, results);
        }

        public async Task<List<CatalogRecord>> GetRecords(GenValuesParams valuesParams)
        {
            String strCmd = "SELECT Id, " + valuesParams.Columns[0];
            for (int i = 1; i < valuesParams.Columns.Length; i++)
                strCmd += ", " + valuesParams.Columns[i];

            strCmd += " FROM [" + valuesParams.TableName + "]";

            if (!String.IsNullOrEmpty(valuesParams.Status))
            {
                if (valuesParams.FilterStatus >= 0)
                    strCmd += " WHERE " + valuesParams.Status + " = " + valuesParams.FilterStatus;
                else
                    strCmd += " WHERE ([" + valuesParams.Status + "] & " + -valuesParams.FilterStatus + ") = " + -valuesParams.FilterStatus;
            }

            if (valuesParams.FilterColumns.Length > 0)
            {
                strCmd += (!String.IsNullOrEmpty(valuesParams.Status) ? " AND " : " WHERE ") + valuesParams.FilterColumns[0] + " = " + valuesParams.FilterValues[0];
                for (int i = 1; i < valuesParams.FilterColumns.Length; i++)
                    strCmd += " AND " + valuesParams.FilterColumns[i] + " = " + valuesParams.FilterValues[i];
            }

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<CatalogRecord> records = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        String[] fields = new String[valuesParams.Columns.Length];
                        for (int i = 0; i < fields.Length; i++)
                            fields[i] = reader[valuesParams.Columns[i]].ToString();

                        String[] filters = new String[valuesParams.FilterColumns.Length];
                        for (int i = 0; i < filters.Length; i++)
                            filters[i] = reader[valuesParams.FilterColumns[i]].ToString();

                        records.Add(new CatalogRecord(long.Parse(reader["Id"].ToString()), fields, filters));
                    }
                }
            }

            return records;
        }

        // GET CODE BY ID
        public async Task<String> GetCodeById(String table, long id)
        {
            String strCmd = $"SELECT Code FROM [{table}]";
            strCmd += " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            String code = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        code = reader["Code"].ToString();
                    }
                }
            }

            return code;
        }

        // GET NAME BY ID
        public async Task<String> GetNameById(String table, long id)
        {
            String strCmd = $"SELECT Name FROM [{table}]";
            strCmd += " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            String name = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        name = reader["Name"].ToString();
                    }
                }
            }

            return name;
        }

        // GET FIELDS
        public async Task<List<String>> GetStrings(String table, String fieldName)
        {
            String strCmd = $"SELECT {fieldName} FROM [{table}]";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<String> fields = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        String field = reader[fieldName].ToString();
                        fields.Add(field);
                    }
                }
            }

            return fields;
        }

        // GET FIELD BY ID
        public async Task<String> GetStringById(String table, long id, String fieldName)
        {
            String strCmd = $"SELECT {fieldName} FROM [{table}]";
            strCmd += " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            String field = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        field = reader[fieldName].ToString();
                    }
                }
            }

            return field;
        }

        public async Task<int> GetIntById(String table, long id, String fieldName)
        {
            String strCmd = $"SELECT {fieldName} FROM [{table}]";
            strCmd += " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            int field = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        field = Convert.ToInt32(reader[fieldName]);
                    }
                }
            }

            return field;
        }

        public async Task<long> GetLongById(String table, long id, String fieldName)
        {
            String strCmd = $"SELECT {fieldName} FROM [{table}]";
            strCmd += " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            long field = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        field = Convert.ToInt64(reader[fieldName]);
                    }
                }
            }

            return field;
        }

        public async Task<String> GetStringByIntFields(String table, String[] keyNames, int[] keyValues, String fieldName)
        {
            String strCmd = $"SELECT {fieldName} FROM [{table}] WHERE";
            strCmd += " @KeyName0 = @KeyValue0";
            for (int i = 1; i < keyNames.Length; i++)
                strCmd += $" AND @KeyName{i} = @KeyValue{i}";

            SqlCommand command = new SqlCommand(strCmd, conn);
            for (int i = 0; i < keyNames.Length; i++)
            {
                DBHelper.AddParam(command, $"@KeyName{i}", SqlDbType.Int, keyNames[i]);
                DBHelper.AddParam(command, $"@KeyValue{i}", SqlDbType.Int, keyValues[i]);
            }

            String field = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        field = reader[fieldName].ToString();
                    }
                }
            }

            return field;
        }

        public async Task<int> GetIntByIntFields(String table, String[] keyNames, int[] keyValues, String fieldName)
        {
            String strCmd = $"SELECT {fieldName} FROM [{table}] WHERE";
            strCmd += " @KeyName0 = @KeyValue0";
            for (int i = 1; i < keyNames.Length; i++)
                strCmd += $" AND @KeyName{i} = @KeyValue{i}";

            SqlCommand command = new SqlCommand(strCmd, conn);
            for (int i = 0; i < keyNames.Length; i++)
            {
                DBHelper.AddParam(command, $"@KeyName{i}", SqlDbType.Int, keyNames[i]);
                DBHelper.AddParam(command, $"@KeyValue{i}", SqlDbType.Int, keyValues[i]);
            }

            int field = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        field = Convert.ToInt32(reader[fieldName]);
                    }
                }
            }

            return field;
        }

        public async Task<long> GetLongByIntFields(String table, String[] keyNames, int[] keyValues, String fieldName)
        {
            String strCmd = $"SELECT {fieldName} FROM [{table}] WHERE";
            strCmd += " @KeyName0 = @KeyValue0";
            for (int i = 1; i < keyNames.Length; i++)
                strCmd += $" AND @KeyName{i} = @KeyValue{i}";

            SqlCommand command = new SqlCommand(strCmd, conn);
            for (int i = 0; i < keyNames.Length; i++)
            {
                DBHelper.AddParam(command, $"@KeyName{i}", SqlDbType.Int, keyNames[i]);
                DBHelper.AddParam(command, $"@KeyValue{i}", SqlDbType.Int, keyValues[i]);
            }

            long field = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        field = Convert.ToInt64(reader[fieldName]);
                    }
                }
            }

            return field;
        }

        // GET ID BY CODE
        public async Task<long> GetIdByCode(String table, String code)
        {
            String strCmd = $"SELECT Id FROM [{table}]";
            strCmd += " WHERE Code = @Code";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, code);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                    }
                }
            }

            return id;
        }

        // GET ID BY FIELD
        public async Task<long> GetIdByField(String table, String fieldName, String fieldValue)
        {
            String strCmd = $"SELECT Id FROM [{table}]";
            strCmd += $" WHERE {fieldName} = @Field";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Field", SqlDbType.VarChar, fieldValue);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                    }
                }
            }

            return id;
        }

        // GET NAME BY CODE
        public async Task<String> GetNameByCode(String table, String code)
        {
            String strCmd = $"SELECT Name FROM [{table}]";
            strCmd += " WHERE Code = @Code";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, code);

            String name = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        name = reader["Name"].ToString();
                    }
                }
            }

            return name;
        }

        // SET
        public async Task<GenValues> Set(GenValuesParams valuesParams)
        {
            String strCmd = "SELECT Id, " + valuesParams.Columns[0];
            for (int i = 1; i < valuesParams.Columns.Length; i++)
                strCmd += ", " + valuesParams.Columns[i];

            strCmd += " FROM [" + valuesParams.TableName + "]";

            if (!String.IsNullOrEmpty(valuesParams.Status))
                strCmd += " WHERE (" + valuesParams.Status + " & @FilterStatus) = @FilterStatus";

            if (!String.IsNullOrEmpty(valuesParams.Status))
            {
                if (valuesParams.FilterStatus >= 0)
                    strCmd += " WHERE " + valuesParams.Status + " = @FilterStatus";
                else
                    strCmd += " WHERE ([" + valuesParams.Status + "] & @NegFilterStatus) = @NegFilterStatus";
            }

            if (valuesParams.FilterColumns != null && valuesParams.FilterColumns.Length > 0)
            {
                if (!String.IsNullOrEmpty(valuesParams.Status))
                    strCmd += " AND " + valuesParams.FilterColumns[0] + " = @FilterValue0";
                else
                    strCmd += " WHERE " + valuesParams.FilterColumns[0] + " = @FilterValue0";

                for (int i = 1; i < valuesParams.FilterColumns.Length; i++)
                    strCmd += " AND " + valuesParams.FilterColumns[i] + " = @FilterValue" + i;
            }

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@FilterStatus", SqlDbType.Int, valuesParams.FilterStatus);
            DBHelper.AddParam(command, "@NegFilterStatus", SqlDbType.Int, -valuesParams.FilterStatus);
            if (valuesParams.FilterColumns != null && valuesParams.FilterColumns.Length > 0)
            {
                for (int i = 0; i < valuesParams.FilterColumns.Length; i++)
                    DBHelper.AddParam(command, "@FilterValue" + i, SqlDbType.VarChar, valuesParams.FilterValues[i]);
            }

            List<String> results = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        String result = reader["Id"].ToString();
                        result += "|" + reader[valuesParams.Columns[0]].ToString();

                        for (int i = 1; i < valuesParams.Columns.Length; i++)
                            result += "^" + reader[valuesParams.Columns[i]].ToString();

                        results.Add(result);
                    }
                }
            }

            return new GenValues(valuesParams.TableName, results);
        }

        public async Task<GenValues> SetParams(GenValuesParams valuesParams)
        {
            String strCmd = "SELECT @Column0";
            for (int i = 1; i < valuesParams.Columns.Length; i++)
                strCmd += ", @Column" + i;

            strCmd += " FROM [@TableName]";

            if (!String.IsNullOrEmpty(valuesParams.Status))
            {
                if (valuesParams.FilterStatus >= 0)
                    strCmd += " WHERE @Status = @FilterStatus";
                else
                    strCmd += " WHERE ([@Status] & @NegFilterStatus) = @NegFilterStatus";
            }

            if (valuesParams.FilterColumns != null && valuesParams.FilterColumns.Length > 0)
            {
                if (!String.IsNullOrEmpty(valuesParams.Status))
                    strCmd += " AND @FilterColumn0 = @FilterValue0";
                else
                    strCmd += " WHERE @FilterColumn0 = @FilterValue0";

                for (int i = 1; i < valuesParams.FilterColumns.Length; i++)
                    strCmd += " AND @FilterColumn" + i + " = @FilterValue" + i;
            }

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@TableName", SqlDbType.VarChar, valuesParams.TableName);
            DBHelper.AddParam(command, "@Status", SqlDbType.VarChar, valuesParams.Status);
            DBHelper.AddParam(command, "@FilterStatus", SqlDbType.Int, valuesParams.FilterStatus);
            DBHelper.AddParam(command, "@NegFilterStatus", SqlDbType.Int, -valuesParams.FilterStatus);

            for (int i = 0; i < valuesParams.Columns.Length; i++)
                DBHelper.AddParam(command, "@Column" + i, SqlDbType.VarChar, valuesParams.Columns[i]);

            if (valuesParams.FilterColumns != null && valuesParams.FilterColumns.Length > 0)
            {
                for (int i = 0; i < valuesParams.FilterColumns.Length; i++)
                {
                    DBHelper.AddParam(command, "@FilterColumn" + i, SqlDbType.VarChar, valuesParams.FilterColumns[i]);
                    DBHelper.AddParam(command, "@FilterValue" + i, SqlDbType.VarChar, valuesParams.FilterValues[i]);
                }
            }

            List<String> results = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        String result = reader["Id"].ToString();
                        result += "|" + reader[valuesParams.Columns[0]].ToString();

                        for (int i = 1; i < valuesParams.Columns.Length; i++)
                            result += "^" + reader[valuesParams.Columns[i]].ToString();

                        results.Add(result);
                    }
                }
            }

            return new GenValues(valuesParams.TableName, results);
        }
    }
}
