using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProductFinancedDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-ProductFinanced]";

        public static ProductFinanced GetProductFinanced(SqlDataReader reader)
        {
            return new ProductFinanced(Convert.ToInt32(reader["Id"]),
                                       Convert.ToInt32(reader["ProjectId"]),
                                       Convert.ToInt32(reader["CpiMin"]),
                                       Convert.ToInt32(reader["CpiMax"]),
                                       Convert.ToInt32(reader["CpiDefault"]),
                                       Convert.ToDouble(reader["AdvRate"]),
                                       Convert.ToDouble(reader["ReserveRate"]),
                                       Convert.ToInt32(reader["OverdueMax"]),
                                       Convert.ToDateTime(reader["CreateDateTime"]),
                                       Convert.ToDateTime(reader["UpdateDateTime"]),
                                       Convert.ToInt32(reader["ProductFinancedStatusId"]));
        }

        // GET
        public async Task<IEnumerable<ProductFinanced>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<ProductFinanced> productsFinanced = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         ProductFinanced productFinanced = GetProductFinanced(reader);
                         productsFinanced.Add(productFinanced);
                    }
                }
            }
            return productsFinanced;
        }

        public async Task<IEnumerable<ProductFinanced>> GetAllByStatus(int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProductFinancedStatusId = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<ProductFinanced> productsFinanced = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ProductFinanced productFinanced = GetProductFinanced(reader);
                        productsFinanced.Add(productFinanced);
                    }
                }
            }
            return productsFinanced;
        }

        public async Task<ProductFinanced> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            ProductFinanced productFinanced = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         productFinanced = GetProductFinanced(reader);
                    }
                }
            }
            return productFinanced;
        }

        public async Task<ProductFinanced> GetByProjectId(int projectId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND ProductFinancedStatusId = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            ProductFinanced productFinanced = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        productFinanced = GetProductFinanced(reader);
                    }
                }
            }
            return productFinanced;
        }

        public async Task<int> GetIdByProjectId(int projectId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE ProjectId = @ProjectId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);

            int id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt32(reader["Id"]);
                    }
                }
            }
            return id;
        }

        public async Task<(double, double)> GetRatesByProjectId(int projectId)
        {
            String strCmd = $"SELECT ReserveRate, AdvRate FROM {table} WHERE ProjectId = @ProjectId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);

            double reserveRate = 0d;
            double advRate = 0d;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        reserveRate = Convert.ToDouble(reader["ReserveRate"]);
                        advRate = Convert.ToDouble(reader["AdvRate"]);
                    }
                }
            }
            return (reserveRate, advRate);
        }

        public async Task<int> GetCpiMinByProjectId(int projectId, int defaultMin = -1)
        {
            String strCmd = $"SELECT CpiMin FROM {table} WHERE ProjectId = @ProjectId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);

            int cpiMin = defaultMin;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cpiMin = Convert.ToInt32(reader["CpiMin"]);
                    }
                }
            }
            return cpiMin;
        }

        // INSERT
        public async Task<int> Add(ProductFinanced productFinanced)
        {
            String strCmd = $"INSERT INTO {table}(ProjectId, CpiMin, CpiMax, CpiDefault, AdvRate, ReserveRate, OverdueMax, CreateDateTime, UpdateDateTime, ProductFinancedStatusId)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ProjectId, @CpiMin, @CpiMax, @CpiDefault, @AdvRate, @ReserveRate, @OverdueMax, @CreateDateTime, @UpdateDateTime, @ProductFinancedStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, productFinanced.ProjectId);
            DBHelper.AddParam(command, "@CpiMin", SqlDbType.Int, productFinanced.CpiMin);
            DBHelper.AddParam(command, "@CpiMax", SqlDbType.Int, productFinanced.CpiMax);
            DBHelper.AddParam(command, "@CpiDefault", SqlDbType.Int, productFinanced.CpiDefault);
            DBHelper.AddParam(command, "@AdvRate", SqlDbType.Decimal, productFinanced.AdvRate);
            DBHelper.AddParam(command, "@ReserveRate", SqlDbType.Decimal, productFinanced.ReserveRate);
            DBHelper.AddParam(command, "@OverdueMax", SqlDbType.Int, productFinanced.OverdueMax);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@ProductFinancedStatusId", SqlDbType.Int, productFinanced.ProductFinancedStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(ProductFinanced productFinanced)
        {
            String strCmd = $"UPDATE {table} SET ProjectId = @ProjectId, CpiMin = @CpiMin, CpiMax = @CpiMax, CpiDefault = @CpiDefault, AdvRate = @AdvRate, ReserveRate = @ReserveRate, OverdueMax = @OverdueMax, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, productFinanced.ProjectId);
            DBHelper.AddParam(command, "@CpiMin", SqlDbType.Int, productFinanced.CpiMin);
            DBHelper.AddParam(command, "@CpiMax", SqlDbType.Int, productFinanced.CpiMax);
            DBHelper.AddParam(command, "@CpiDefault", SqlDbType.Int, productFinanced.CpiDefault);
            DBHelper.AddParam(command, "@AdvRate", SqlDbType.Decimal, productFinanced.AdvRate);
            DBHelper.AddParam(command, "@ReserveRate", SqlDbType.Decimal, productFinanced.ReserveRate);
            DBHelper.AddParam(command, "@OverdueMax", SqlDbType.Int, productFinanced.OverdueMax);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, productFinanced.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, ProductFinancedStatusId = @ProductFinancedStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@ProductFinancedStatusId", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

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
    }
}
