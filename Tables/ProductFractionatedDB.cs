using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProductFractionatedDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-ProductFractionated]";

        public static ProductFractionated GetProductFractionated(SqlDataReader reader)
        {
            return new ProductFractionated(Convert.ToInt32(reader["Id"]),
                                          Convert.ToInt32(reader["ProjectId"]),
                                          Convert.ToInt32(reader["CpiMin"]),
                                          Convert.ToInt32(reader["CpiMax"]),
                                          Convert.ToInt32(reader["CpiDefault"]),
                                          Convert.ToDouble(reader["ReserveRate"]),
                                          Convert.ToInt32(reader["OverdueMax"]),
                                          Convert.ToDateTime(reader["CreateDateTime"]),
                                          Convert.ToDateTime(reader["UpdateDateTime"]),
                                          Convert.ToInt32(reader["ProductFractionatedStatusId"]));
        }

        // GET
        public async Task<IEnumerable<ProductFractionated>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<ProductFractionated> productsFractionated = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         ProductFractionated productFractionated = GetProductFractionated(reader);
                         productsFractionated.Add(productFractionated);
                    }
                }
            }
            return productsFractionated;
        }

        public async Task<IEnumerable<ProductFractionated>> GetAllByStatus(int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProductFractionatedStatusId = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<ProductFractionated> productsFractionated = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ProductFractionated productFractionated = GetProductFractionated(reader);
                        productsFractionated.Add(productFractionated);
                    }
                }
            }
            return productsFractionated;
        }

        public async Task<ProductFractionated> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            ProductFractionated productFractionated = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         productFractionated = GetProductFractionated(reader);
                    }
                }
            }
            return productFractionated;
        }

        public async Task<ProductFractionated> GetByProjectId(int projectId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND ProductFractionatedStatusId = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            ProductFractionated productFractionated = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        productFractionated = GetProductFractionated(reader);
                    }
                }
            }
            return productFractionated;
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

        public async Task<double> GetReserveRateByProjectId(int projectId)
        {
            String strCmd = $"SELECT ReserveRate FROM {table} WHERE ProjectId = @ProjectId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);

            double reserveRate = 0d;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        reserveRate = Convert.ToDouble(reader["ReserveRate"]);
                    }
                }
            }
            return reserveRate;
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
        public async Task<int> Add(ProductFractionated productFractionated)
        {
            String strCmd = $"INSERT INTO {table}(ProjectId, CpiMin, CpiMax, CpiDefault, ReserveRate, OverdueMax, CreateDateTime, UpdateDateTime, ProductFractionatedStatusId)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ProjectId, @CpiMin, @CpiMax, @CpiDefault, @ReserveRate, @OverdueMax, @CreateDateTime, @UpdateDateTime, @ProductFractionatedStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, productFractionated.ProjectId);
            DBHelper.AddParam(command, "@CpiMin", SqlDbType.Int, productFractionated.CpiMin);
            DBHelper.AddParam(command, "@CpiMax", SqlDbType.Int, productFractionated.CpiMax);
            DBHelper.AddParam(command, "@CpiDefault", SqlDbType.Int, productFractionated.CpiDefault);
            DBHelper.AddParam(command, "@ReserveRate", SqlDbType.Decimal, productFractionated.ReserveRate);
            DBHelper.AddParam(command, "@OverdueMax", SqlDbType.Int, productFractionated.OverdueMax);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@ProductFractionatedStatusId", SqlDbType.Int, productFractionated.ProductFractionatedStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(ProductFractionated productFractionated)
        {
            String strCmd = $"UPDATE {table} SET ProjectId = @ProjectId, CpiMin = @CpiMin, CpiMax = @CpiMax, CpiDefault = @CpiDefault, ReserveRate = @ReserveRate, OverdueMax = @OverdueMax, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, productFractionated.ProjectId);
            DBHelper.AddParam(command, "@CpiMin", SqlDbType.Int, productFractionated.CpiMin);
            DBHelper.AddParam(command, "@CpiMax", SqlDbType.Int, productFractionated.CpiMax);
            DBHelper.AddParam(command, "@CpiDefault", SqlDbType.Int, productFractionated.CpiDefault);
            DBHelper.AddParam(command, "@ReserveRate", SqlDbType.Decimal, productFractionated.ReserveRate);
            DBHelper.AddParam(command, "@OverdueMax", SqlDbType.Int, productFractionated.OverdueMax);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, productFractionated.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, ProductFractionatedStatusId = @ProductFractionatedStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@ProductFractionatedStatusId", SqlDbType.Int, status);
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
