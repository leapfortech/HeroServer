using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class CpiRangeDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-CpiRange]";

        public static CpiRange GetCpiRange(SqlDataReader reader)
        {
            return new CpiRange(Convert.ToInt32(reader["Id"]),
                                Convert.ToInt32(reader["ProjectId"]),
                                Convert.ToInt32(reader["ProductTypeId"]),
                                Convert.ToInt32(reader["AmountMin"]),
                                Convert.ToInt32(reader["AmountMax"]),
                                Convert.ToDouble(reader["DiscountRate"]),
                                Convert.ToDouble(reader["ProfitablityRate"]),
                                Convert.ToDateTime(reader["CreateDateTime"]),
                                Convert.ToDateTime(reader["UpdateDateTime"]),
                                Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<CpiRange>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<CpiRange> cpiRanges = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         CpiRange cpiRange = GetCpiRange(reader);
                         cpiRanges.Add(cpiRange);
                    }
                }
            }
            return cpiRanges;
        }

        public async Task<CpiRange> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            CpiRange cpiRange = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         cpiRange = GetCpiRange(reader);
                    }
                }
            }
            return cpiRange;
        }

        public async Task<List<CpiRange>> GetByProjectId(int projectId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<CpiRange> cpiRanges = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        CpiRange cpiRange = GetCpiRange(reader);
                        cpiRanges.Add(cpiRange);
                    }
                }
            }
            return cpiRanges;
        }

        public async Task<List<CpiRange>> GetByProjectProductId(int projectId, int productTypeId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND ProductTypeId = @ProductTypeId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@ProductTypeId", SqlDbType.Int, productTypeId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<CpiRange> cpiRanges = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        CpiRange cpiRange = GetCpiRange(reader);
                        cpiRanges.Add(cpiRange);
                    }
                }
            }
            return cpiRanges;
        }


        // INSERT
        public async Task<int> Add(CpiRange cpiRange)
        {
            String strCmd = $"INSERT INTO {table}(ProjectId, ProductTypeId, AmountMin, AmountMax, DiscountRate, ProfitablityRate, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ProjectId, @ProductTypeId, @AmountMin, @AmountMax, @DiscountRate, @ProfitablityRate, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, cpiRange.ProjectId);
            DBHelper.AddParam(command, "@ProductTypeId", SqlDbType.Int, cpiRange.ProductTypeId);
            DBHelper.AddParam(command, "@AmountMin", SqlDbType.Int, cpiRange.AmountMin);
            DBHelper.AddParam(command, "@AmountMax", SqlDbType.Int, cpiRange.AmountMax);
            DBHelper.AddParam(command, "@DiscountRate", SqlDbType.Decimal, cpiRange.DiscountRate);
            DBHelper.AddParam(command, "@ProfitablityRate", SqlDbType.Decimal, cpiRange.ProfitablityRate);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, cpiRange.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(CpiRange cpiRange, bool status = false)
        {
            String strCmd = $"UPDATE {table} SET ProjectId = @ProjectId, ProductTypeId = @ProductTypeId, AmountMin = @AmountMin, AmountMax = @AmountMax, DiscountRate = @DiscountRate," +
                             " ProfitablityRate = @ProfitablityRate, UpdateDateTime = @UpdateDateTime" +
                             (status ? ", Status = @Status" : "") +
                             " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, cpiRange.ProjectId);
            DBHelper.AddParam(command, "@ProductTypeId", SqlDbType.Int, cpiRange.ProductTypeId);
            DBHelper.AddParam(command, "@AmountMin", SqlDbType.Int, cpiRange.AmountMin);
            DBHelper.AddParam(command, "@AmountMax", SqlDbType.Int, cpiRange.AmountMax);
            DBHelper.AddParam(command, "@DiscountRate", SqlDbType.Decimal, cpiRange.DiscountRate);
            DBHelper.AddParam(command, "@ProfitablityRate", SqlDbType.Decimal, cpiRange.ProfitablityRate);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            if (status)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, cpiRange.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, cpiRange.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByProjectId(int projectId, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE ProjectId = @ProjectId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        // DELETE
        public async Task<int> DeleteAll()
        {
            String strCmd = $"DELETE {table}";
            SqlCommand command = new SqlCommand(strCmd, conn);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteById(int id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeleteByProjectId(int projectId)
        {
            String strCmd = $"DELETE {table} WHERE ProjectId = @ProjectId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
