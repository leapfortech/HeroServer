using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProductPrepaidDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-ProductPrepaid]";

        public static ProductPrepaid GetProductPrepaid(SqlDataReader reader)
        {
            return new ProductPrepaid(Convert.ToInt32(reader["Id"]),
                                      Convert.ToInt32(reader["ProjectId"]),
                                      Convert.ToInt32(reader["CpiMin"]),
                                      Convert.ToInt32(reader["CpiMax"]),
                                      Convert.ToInt32(reader["CpiDefault"]),
                                      Convert.ToDouble(reader["ReserveRate"]),
                                      Convert.ToDateTime(reader["CreateDateTime"]),
                                      Convert.ToDateTime(reader["UpdateDateTime"]),
                                      Convert.ToInt32(reader["ProductPrepaidStatusId"]));
        }

        // GET
        public async Task<IEnumerable<ProductPrepaid>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<ProductPrepaid> productPrepaids = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         ProductPrepaid productPrepaid = GetProductPrepaid(reader);
                         productPrepaids.Add(productPrepaid);
                    }
                }
            }
            return productPrepaids;
        }

        public async Task<IEnumerable<ProductPrepaid>> GetAllByStatus(int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProductPrepaidStatusId = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<ProductPrepaid> productPrepaids = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ProductPrepaid productPrepaid = GetProductPrepaid(reader);
                        productPrepaids.Add(productPrepaid);
                    }
                }
            }
            return productPrepaids;
        }

        public async Task<ProductPrepaid> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            ProductPrepaid productPrepaid = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         productPrepaid = GetProductPrepaid(reader);
                    }
                }
            }
            return productPrepaid;
        }

        public async Task<ProductPrepaid> GetByProjectId(int projectId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND ProductPrepaidStatusId = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            ProductPrepaid productPrepaid = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        productPrepaid = GetProductPrepaid(reader);
                    }
                }
            }
            return productPrepaid;
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
        public async Task<int> Add(ProductPrepaid productPrepaid)
        {
            String strCmd = $"INSERT INTO {table}(ProjectId, CpiMin, CpiMax, CpiDefault, ReserveRate, CreateDateTime, UpdateDateTime, ProductPrepaidStatusId)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ProjectId, @CpiMin, @CpiMax, @CpiDefault, @ReserveRate, @CreateDateTime, @UpdateDateTime, @ProductPrepaidStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, productPrepaid.ProjectId);
            DBHelper.AddParam(command, "@CpiMin", SqlDbType.Int, productPrepaid.CpiMin);
            DBHelper.AddParam(command, "@CpiMax", SqlDbType.Int, productPrepaid.CpiMax);
            DBHelper.AddParam(command, "@CpiDefault", SqlDbType.Int, productPrepaid.CpiDefault);
            DBHelper.AddParam(command, "@ReserveRate", SqlDbType.Decimal, productPrepaid.ReserveRate);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@ProductPrepaidStatusId", SqlDbType.Int, productPrepaid.ProductPrepaidStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(ProductPrepaid productPrepaid)
        {
            String strCmd = $"UPDATE {table} SET ProjectId = @ProjectId, CpiMin = @CpiMin, CpiMax = @CpiMax, CpiDefault = @CpiDefault, ReserveRate = @ReserveRate, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, productPrepaid.ProjectId);
            DBHelper.AddParam(command, "@CpiMin", SqlDbType.Int, productPrepaid.CpiMin);
            DBHelper.AddParam(command, "@CpiMax", SqlDbType.Int, productPrepaid.CpiMax);
            DBHelper.AddParam(command, "@CpiDefault", SqlDbType.Int, productPrepaid.CpiDefault);
            DBHelper.AddParam(command, "@ReserveRate", SqlDbType.Decimal, productPrepaid.ReserveRate);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, productPrepaid.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, ProductPrepaidStatusId = @ProductPrepaidStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@ProductPrepaidStatusId", SqlDbType.Int, status);
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
