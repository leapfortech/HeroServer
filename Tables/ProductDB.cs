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

        public static ProductFull GetProductFull(SqlDataReader reader)
        {
            return new ProductFull(Convert.ToInt64(reader["Id"]),

                                   Convert.ToInt64(reader["PostId"]),
                                   Convert.ToInt64(reader["AppUserId"]),
                                   reader["AppUserAlias"].ToString(),
                                   Convert.ToInt64(reader["PostTypeId"]),
                                   Convert.ToInt64(reader["PostSubtypeId"]),
                                   Convert.ToInt64(reader["PostOriginCountryId"]),
                                   Convert.ToInt64(reader["PostOriginStateId"]),
                                   reader["Title"].ToString(),
                                   reader["Summary"].ToString(),
                                   reader["Description"].ToString(),
                                   Convert.ToInt32(reader["ImageCount"]),
                                   Convert.ToInt32(reader["LikesCount"]),
                                   Convert.ToDateTime(reader["PublicationDateTime"]),
                                   Convert.ToInt32(reader["PostStatus"]),

                                   Convert.ToInt64(reader["OriginCountryId"]),
                                   Convert.ToInt64(reader["SaleCountryId"]),
                                   Convert.ToInt64(reader["SaleStateId"]),
                                   Convert.ToInt64(reader["CurrencyId"]),
                                   Convert.ToDouble(reader["Price"]),
                                   Convert.ToDouble(reader["DiscountPrice"]),
                                   Convert.ToInt32(reader["Status"]),

                                   null,    // ContactFull
                                   null);   // ProductReviewFull
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

        // GET FULL
        public async Task<ProductFull> GetFullById(long id)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId, Post.PostSubtypeId," +
                             " Post.PostOriginCountryId, Post.PostOriginStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikesCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.OriginCountryId, {table}.SaleCountryId, {table}.SaleStateId," +
                            $" {table}.CurrencyId, {table}.Price, {table}.DiscountPrice, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.Id = @Id;";

            strCmd += "SELECT Id, Name, PhoneCountryId, Phone, Email" +
                      " FROM [D-Contact]" +
                      " WHERE Status = 1 AND ProductId = @Id;";

            strCmd += "SELECT ProductReview.Id, ProductReview.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " ProductReview.Rating, ProductReview.Description, ProductReview.Status" +
                      " FROM [D-ProductReview] AS ProductReview" +
                      " INNER JOIN [D-AppUser] AS AppUser ON (ProductReview.AppUserId = AppUser.Id)" +
                      " WHERE ProductReview.Status = 1 AND ProductReview.ProductId = @Id;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            ProductFull productFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    productFull = GetProductFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        productFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    productFull.ProductReviewFulls = [];
                    while (await reader.ReadAsync())
                    {
                        ProductReviewFull ProductReviewFull = ProductReviewDB.GetProductReviewFull(reader);
                        productFull.ProductReviewFulls.Add(ProductReviewFull);
                    }
                }
            }

            return productFull;
        }

        public async Task<ProductFull> GetFullByPostId(long postId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId, Post.PostSubtypeId," +
                             " Post.PostOriginCountryId, Post.PostOriginStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikesCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.OriginCountryId, {table}.SaleCountryId, {table}.SaleStateId," +
                            $" {table}.CurrencyId, {table}.Price, {table}.DiscountPrice, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd += "SELECT Id, Name, PhoneCountryId, Phone, Email" +
                      " FROM [D-Contact]" +
                      " WHERE Status = 1 AND ProductId IN" +
                      $" (SELECT Id FROM {table} WHERE PostId = @PostId);";

            strCmd += "SELECT ProductReview.Id, ProductReview.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " ProductReview.Rating, ProductReview.Description, ProductReview.Status" +
                      " FROM [D-ProductReview] AS ProductReview" +
                      " LEFT JOIN [D-AppUser] AS AppUser ON (ProductReview.AppUserId = AppUser.Id)" +
                      " WHERE ProductReview.Status = 1 AND ProductReview.ProductId IN" +
                      $" (SELECT Id FROM {table} WHERE PostId = @PostId);";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

            ProductFull productFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    productFull = GetProductFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        productFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    productFull.ProductReviewFulls = [];
                    while (await reader.ReadAsync())
                    {
                        ProductReviewFull productReviewFull = ProductReviewDB.GetProductReviewFull(reader);
                        productFull.ProductReviewFulls.Add(productReviewFull);
                    }
                }
            }

            return productFull;
        }

        public async Task<ProductDataFull> GetFullsByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId, Post.PostSubtypeId," +
                             " Post.PostOriginCountryId, Post.PostOriginStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikesCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.OriginCountryId, {table}.SaleCountryId, {table}.SaleStateId, {table}.CurrencyId," +
                            $" {table}.Price, {table}.DiscountPrice, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)";

            if (status != -1)
                strCmd += $" WHERE {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd +=  "SELECT Contact.Id, Contact.Name, Contact.PhoneCountryId, Contact.Phone," +
                       " Contact.Email, Contact.Status, Contact.ProductId" +
                       " FROM [D-Contact] AS Contact" +
                      $" JOIN {table} ON (Contact.ProductId = {table}.Id)" +
                       " WHERE Contact.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd +=  "SELECT ProductReview.Id, ProductReview.AppUserId, AppUser.Alias AS AppUserAlias," +
                       " ProductReview.Rating, ProductReview.Description, ProductReview.Status, ProductReview.ProductId" +
                       " FROM [D-ProductReview] AS ProductReview" +
                      $" JOIN {table} ON (ProductReview.ProductId = {table}.Id)" +
                       " JOIN [D-AppUser] AS AppUser ON (ProductReview.AppUserId = AppUser.Id)" +
                       " WHERE ProductReview.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";


            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            ProductDataFull productDataFull = new ProductDataFull();
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<ProductFull> productFulls = [];
                    while (await reader.ReadAsync())
                    {
                        ProductFull product = GetProductFull(reader);
                        productFulls.Add(product);
                    }
                    productDataFull.ProductFulls = productFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contacts = [];
                    while (await reader.ReadAsync())
                    {
                        ContactFull contact = ContactDB.GetContactFull(reader);
                        contacts.Add(contact);
                    }
                    productDataFull.ContactFulls = contacts;

                    await reader.NextResultAsync();
                    List<ProductReviewFull> reviews = [];
                    while (await reader.ReadAsync())
                    {
                        ProductReviewFull ProductReviewFull = ProductReviewDB.GetProductReviewFull(reader);
                        reviews.Add(ProductReviewFull);
                    }
                    productDataFull.ProductReviewFulls = reviews;
                }
            }

            return productDataFull;
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
