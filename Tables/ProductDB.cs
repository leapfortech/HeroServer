using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProductDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Product]";

        private static Product GetProduct(SqlDataReader reader)
        {
            return new Product(Convert.ToInt64(reader["Id"]),
                               Convert.ToInt64(reader["PostId"]),
                               Convert.ToInt64(reader["OriginCountryId"]),
                               Convert.ToInt64(reader["SaleCountryId"]),
                               Convert.ToInt64(reader["SaleStateId"]),
                               Convert.ToInt64(reader["CurrencyId"]),
                               Convert.ToDouble(reader["Price"]),
                               Convert.ToDouble(reader["DiscountPrice"]),
                               Convert.ToInt64(reader["ContactIdentityId"]),
                               Convert.ToDateTime(reader["CreateDateTime"]),
                               Convert.ToDateTime(reader["UpdateDateTime"]),
                               Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Product>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Product> products = new List<Product>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Product product = GetProduct(reader);
                         products.Add(product);
                    }
                }
            }
            return products;
        }

        public async Task<Product> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Product product = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         product = GetProduct(reader);
                    }
                }
            }
            return product;
        }

        // INSERT
        public async Task<long> Add(Product product)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, OriginCountryId, SaleCountryId, SaleStateId, CurrencyId, Price, DiscountPrice, ContactIdentityId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @OriginCountryId, @SaleCountryId, @SaleStateId, @CurrencyId, @Price, @DiscountPrice, @ContactIdentityId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, product.PostId);
            DBHelper.AddParam(command, "@OriginCountryId", SqlDbType.BigInt, product.OriginCountryId);
            DBHelper.AddParam(command, "@SaleCountryId", SqlDbType.BigInt, product.SaleCountryId);
            DBHelper.AddParam(command, "@SaleStateId", SqlDbType.BigInt, product.SaleStateId);
            DBHelper.AddParam(command, "@CurrencyId", SqlDbType.BigInt, product.CurrencyId);
            DBHelper.AddParam(command, "@Price", SqlDbType.Decimal, product.Price);
            DBHelper.AddParam(command, "@DiscountPrice", SqlDbType.Decimal, product.DiscountPrice);
            DBHelper.AddParam(command, "@ContactIdentityId", SqlDbType.BigInt, product.ContactIdentityId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, product.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Product product)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, OriginCountryId = @OriginCountryId, SaleCountryId = @SaleCountryId, SaleStateId = @SaleStateId, CurrencyId = @CurrencyId, Price = @Price, DiscountPrice = @DiscountPrice, ContactIdentityId = @ContactIdentityId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, product.PostId);
            DBHelper.AddParam(command, "@OriginCountryId", SqlDbType.BigInt, product.OriginCountryId);
            DBHelper.AddParam(command, "@SaleCountryId", SqlDbType.BigInt, product.SaleCountryId);
            DBHelper.AddParam(command, "@SaleStateId", SqlDbType.BigInt, product.SaleStateId);
            DBHelper.AddParam(command, "@CurrencyId", SqlDbType.BigInt, product.CurrencyId);
            DBHelper.AddParam(command, "@Price", SqlDbType.Decimal, product.Price);
            DBHelper.AddParam(command, "@DiscountPrice", SqlDbType.Decimal, product.DiscountPrice);
            DBHelper.AddParam(command, "@ContactIdentityId", SqlDbType.BigInt, product.ContactIdentityId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, product.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, product.Id);

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
