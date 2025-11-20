using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProductReviewDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-ProductReview]";

        private static ProductReview GetProductReview(SqlDataReader reader)
        {
            return new ProductReview(Convert.ToInt64(reader["Id"]),
                                     Convert.ToInt64(reader["ProductId"]),
                                     Convert.ToInt64(reader["AppUserId"]),
                                     Convert.ToInt32(reader["Rating"]),
                                     reader["Description"].ToString(),
                                     Convert.ToDateTime(reader["CreateDateTime"]),
                                     Convert.ToDateTime(reader["UpdateDateTime"]),
                                     Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<ProductReview>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<ProductReview> productReviews = new List<ProductReview>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         ProductReview productReview = GetProductReview(reader);
                         productReviews.Add(productReview);
                    }
                }
            }
            return productReviews;
        }

        public async Task<ProductReview> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            ProductReview productReview = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         productReview = GetProductReview(reader);
                    }
                }
            }
            return productReview;
        }

        // INSERT
        public async Task<long> Add(ProductReview productReview)
        {
            String strCmd = $"INSERT INTO {table}(Id, ProductId, AppUserId, Rating, Description, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @ProductId, @AppUserId, @Rating, @Description, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@ProductId", SqlDbType.BigInt, productReview.ProductId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, productReview.AppUserId);
            DBHelper.AddParam(command, "@Rating", SqlDbType.Int, productReview.Rating);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, productReview.Description);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, productReview.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(ProductReview productReview)
        {
            String strCmd = $"UPDATE {table} SET ProductId = @ProductId, AppUserId = @AppUserId, Rating = @Rating, Description = @Description, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProductId", SqlDbType.BigInt, productReview.ProductId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, productReview.AppUserId);
            DBHelper.AddParam(command, "@Rating", SqlDbType.Int, productReview.Rating);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, productReview.Description);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, productReview.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, productReview.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

        public async Task<bool> DeleteById(long id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
