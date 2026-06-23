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
                               Convert.ToInt64(reader["ProductSubtypeId"]),
                               Convert.ToInt64(reader["SaleCountryId"]),
                               Convert.ToInt64(reader["SaleStateId"]),
                               Convert.ToInt64(reader["CurrencyId"]),
                               Convert.ToDouble(reader["Price"]),
                               Convert.ToDouble(reader["DiscountPrice"]),
                               Convert.ToInt64(reader["DeliveryTypeId"]),
                               reader["Annotation"].ToString(),
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
                                   Convert.ToInt64(reader["PostCountryId"]),
                                   Convert.ToInt64(reader["PostStateId"]),
                                   reader["Title"].ToString(),
                                   null,   //TitleImage
                                   reader["Summary"].ToString(),
                                   reader["Description"].ToString(),
                                   Convert.ToInt32(reader["ImageCount"]),
                                   Convert.ToInt32(reader["Favorite"]),
                                   Convert.ToInt32(reader["Like"]),
                                   Convert.ToInt32(reader["LikeCount"]),
                                   Convert.ToDateTime(reader["PublicationDateTime"]),
                                   Convert.ToInt32(reader["Status"]),
                                   null,   //ContactFull
                                   null,   //LinkFulls
                                   null,   //CommentFulls

                                   Convert.ToInt64(reader["ProductSubtypeId"]),
                                   Convert.ToInt64(reader["SaleCountryId"]),
                                   Convert.ToInt64(reader["SaleStateId"]),
                                   Convert.ToInt64(reader["CurrencyId"]),
                                   Convert.ToDouble(reader["Price"]),
                                   Convert.ToDouble(reader["DiscountPrice"]),
                                   Convert.ToInt64(reader["DeliveryTypeId"]),
                                   reader["Annotation"].ToString(),
                                   Convert.ToInt32(reader["Status"]),

                                   null,   // ProductReviewFull
                                   null);  //Images);
        }


        // GET
        public async Task<List<Product>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<Product> products = [];
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

            command.AddParam("@Id", SqlDbType.BigInt, id);

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

        public async Task<long> GetIdByPostId(long postId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE PostId = @PostId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);

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

        // GET FULL
        public async Task<ProductFull> GetFullById(long id, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(Lik.[Rank], -1) AS [Like]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.ProductSubtypeId, {table}.SaleCountryId, {table}.SaleStateId," +
                            $" {table}.CurrencyId, {table}.Price, {table}.DiscountPrice, {table}.DeliveryTypeId, {table}.Annotation," + 
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                            " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId " +
                            $" WHERE {table}.Id = @Id;";

            strCmd += "SELECT ProductReview.Id, ProductReview.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " ProductReview.Rating, ProductReview.Description, ProductReview.Status" +
                      " FROM [D-ProductReview] AS ProductReview" +
                      " INNER JOIN [D-AppUser] AS AppUser ON (ProductReview.AppUserId = AppUser.Id)" +
                      " WHERE ProductReview.Status = 1 AND ProductReview.ProductId = @Id;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                      $" WHERE Status = 1 AND PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                       " FROM [D-Link] AS Link" +
                      $" WHERE Link.Status = 1 AND Link.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON (Comment.AppUserId = AppUser.Id)" +
                     $" WHERE Comment.Status = 1 AND Comment.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            SqlCommand command = new SqlCommand(strCmd, conn);
            command.AddParam("@Id", SqlDbType.BigInt, id);
            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

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
                    productFull.ProductReviewFulls = [];
                    while (await reader.ReadAsync())
                    {
                        ProductReviewFull ProductReviewFull = ProductReviewDB.GetProductReviewFull(reader);
                        productFull.ProductReviewFulls.Add(ProductReviewFull);
                    }

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        productFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    productFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    productFull.CommentFulls = commentFulls;
                }
            }

            return productFull;
        }

        public async Task<ProductFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(Lik.[Rank], -1) AS [Like]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.ProductSubtypeId, {table}.SaleCountryId, {table}.SaleStateId," +
                            $" {table}.CurrencyId, {table}.Price, {table}.DiscountPrice, {table}.DeliveryTypeId, {table}.Annotation," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                            " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId " +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd += "SELECT ProductReview.Id, ProductReview.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " ProductReview.Rating, ProductReview.Description, ProductReview.Status" +
                      " FROM [D-ProductReview] AS ProductReview" +
                      " LEFT JOIN [D-AppUser] AS AppUser ON (ProductReview.AppUserId = AppUser.Id)" +
                      " WHERE ProductReview.Status = 1 AND ProductReview.ProductId IN" +
                      $" (SELECT Id FROM {table} WHERE PostId = @PostId);";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                       " WHERE Status = 1 AND PostId = @PostId;";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
              " FROM [D-Link] AS Link" +
              " WHERE Link.Status = 1 AND Link.PostId = @PostId;";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                      " WHERE Comment.Status = 1 AND Comment.PostId = @PostId;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

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
                    productFull.ProductReviewFulls = [];
                    while (await reader.ReadAsync())
                    {
                        ProductReviewFull productReviewFull = ProductReviewDB.GetProductReviewFull(reader);
                        productFull.ProductReviewFulls.Add(productReviewFull);
                    }

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        productFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    productFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    productFull.CommentFulls = commentFulls;
                }
            }

            return productFull;
        }

        public async Task<ProductDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, 0 AS Favorite, -1 AS [Like], Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.ProductSubtypeId, {table}.SaleCountryId, {table}.SaleStateId, {table}.CurrencyId," +
                            $" {table}.Price, {table}.DiscountPrice, {table}.DeliveryTypeId, {table}.Annotation," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)";

            if (status != -1)
                strCmd += $" WHERE {table}.Status = @Status;";
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

            strCmd += "SELECT Contact.Id, Contact.PostId, Contact.Name, Contact.Status" +
                      " FROM [D-Contact] AS Contact" +
                      $" INNER JOIN {table} ON (Contact.PostId = {table}.PostId)" +
                       " WHERE Contact.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                       " FROM [D-Link] AS Link" +
                      $" INNER JOIN {table} ON (Link.PostId = {table}.PostId)" +
                       " WHERE Link.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                       " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                       " FROM [D-Comment] AS Comment" +
                       " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                      $" INNER JOIN {table}" +
                      $" ON (Comment.PostId = {table}.PostId)" +
                       " WHERE Comment.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";


            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            ProductDataFull productDataFull = new ProductDataFull();
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<ProductFull> productFulls = [];
                    while (await reader.ReadAsync())
                        productFulls.Add(GetProductFull(reader));
                    productDataFull.ProductFulls = productFulls;

                    await reader.NextResultAsync();
                    List<ProductReviewFull> reviews = [];
                    while (await reader.ReadAsync())
                        reviews.Add(ProductReviewDB.GetProductReviewFull(reader));
                    productDataFull.ProductReviewFulls = reviews;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    productDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    productDataFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    productDataFull.CommentFulls = commentFulls;
                }
            }

            return productDataFull;
        }

        // INSERT
        public async Task<long> Add(Product product)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, ProductSubtypeId, SaleCountryId, SaleStateId, CurrencyId, Price, DiscountPrice, DeliveryTypeId, Annotation, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @ProductSubtypeId, @SaleCountryId, @SaleStateId, @CurrencyId, @Price, @DiscountPrice, @DeliveryTypeId, @Annotation, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('U'));
            command.AddParam("@PostId", SqlDbType.BigInt, product.PostId);
            command.AddParam("@ProductSubtypeId", SqlDbType.BigInt, product.ProductSubtypeId);
            command.AddParam("@SaleCountryId", SqlDbType.BigInt, product.SaleCountryId);
            command.AddParam("@SaleStateId", SqlDbType.BigInt, product.SaleStateId);
            command.AddParam("@CurrencyId", SqlDbType.BigInt, product.CurrencyId);
            command.AddParam("@Price", SqlDbType.Decimal, product.Price);
            command.AddParam("@DiscountPrice", SqlDbType.Decimal, product.DiscountPrice);
            command.AddParam("@DeliveryTypeId", SqlDbType.BigInt, product.DeliveryTypeId);
            command.AddParam("@Annotation", SqlDbType.VarChar, product.Annotation);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, product.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Product product)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, ProductSubtypeId = @ProductSubtypeId, SaleCountryId = @SaleCountryId, SaleStateId = @SaleStateId, CurrencyId = @CurrencyId, Price = @Price, DiscountPrice = @DiscountPrice, DeliveryTypeId = @DeliveryTypeId, Annotation = @Annotation, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, product.PostId);
            command.AddParam("@ProductSubtypeId", SqlDbType.BigInt, product.ProductSubtypeId);
            command.AddParam("@SaleCountryId", SqlDbType.BigInt, product.SaleCountryId);
            command.AddParam("@SaleStateId", SqlDbType.BigInt, product.SaleStateId);
            command.AddParam("@CurrencyId", SqlDbType.BigInt, product.CurrencyId);
            command.AddParam("@Price", SqlDbType.Decimal, product.Price);
            command.AddParam("@DiscountPrice", SqlDbType.Decimal, product.DiscountPrice);
            command.AddParam("@DeliveryTypeId", SqlDbType.BigInt, product.DeliveryTypeId);
            command.AddParam("@Annotation", SqlDbType.VarChar, product.Annotation);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, product.Status);
            command.AddParam("@Id", SqlDbType.BigInt, product.Id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE PostId = @PostId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);
            command.AddParam("@PostId", SqlDbType.BigInt, postId);

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

            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeleteByPostId(long postId)
        {
            String strCmd = $"DELETE {table} WHERE PostId = @PostId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
