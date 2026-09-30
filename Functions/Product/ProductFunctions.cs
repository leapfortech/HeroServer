using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class ProductFunctions
    {
        // GET
        public static async Task<List<Product>> GetAllByStatus(int status)
        {
            return await new ProductDB().GetAllByStatus(status);
        }

        public static async Task<Product> GetById(long id)
        {
            return await new ProductDB().GetById(id);
        }

        public static async Task<ProductFull> GetFullById(long id, long reactionAppUserId)
        {
            ProductFull productFull = await new ProductDB().GetFullById(id, reactionAppUserId);

            if (productFull == null)
                return null;

            productFull.Images = await PostFunctions.GetImagesById(productFull.PostId, true);

            return productFull;
        }

        public static async Task<ProductFull> GetFullByPostId(long postId, long reactionAppUserId)
        {
            ProductFull productFull = await new ProductDB().GetFullByPostId(postId, reactionAppUserId);

            if (productFull == null)
                return null;

            productFull.Images = await PostFunctions.GetImagesById(productFull.PostId, true);

            return productFull;
        }

        public static async Task<List<ProductFull>> GetFullsByStatus(int status)
        {
            ProductDataFull productDataFull = await new ProductDB().GetDataFullByStatus(status);

            return await GetFulls(productDataFull);
        }

        public static async Task<List<ProductFull>> GetFulls(ProductDataFull productDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in productDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in productDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in productDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // ProductFull
            List<ProductFull> productFulls = [];
            foreach (ProductFull productFull in productDataFull.ProductFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(productFull.PostId, out ContactFull contact))
                    contact = null;

                productFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(productFull.PostId, out List<LinkFull> links))
                    links = [];

                productFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(productFull.PostId, out List<CommentFull> comments))
                    comments = [];

                productFull.CommentFulls = comments;

                // Images
                productFull.Images = await PostFunctions.GetImagesById(productFull.PostId, true);

                productFulls.Add(productFull);
            }

            return productFulls;
        }

        // FEED
        public static async Task<ProductFeedResponse> GetFeed(ProductFeedRequest request)
        {
            ProductFeedResponse response = await new ProductDB().GetFeed(request);

            // TitleImages
            List<Task<String>> tasks = [];
            for (int i = 0; i < response.ProductFeeds.Count; i++)
                tasks.Add(PostFunctions.GetTitleImageByPostId(response.ProductFeeds[i].PostId));

            String[] images = await Task.WhenAll(tasks);

            for (int i = 0; i < response.ProductFeeds.Count; i++)
                response.ProductFeeds[i].TitleImage = images[i];

            return response;
        }

        // REGISTER
        public static async Task<long> Register(ProductFull productFull)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                productFull.PostTypeId = PostType.Product;
                productFull.PostId = await PostFunctions.Register(productFull);
                productFull.Status = 1;

                id = await Add(new Product(productFull));

                scope.Complete();
            }

            return id;
        }

        public static async Task<long> RegisterReview(ProductReview productReview)
        {
                productReview.Status = 1;
                return await new ProductReviewDB().Add(productReview);
        }

        // ADD
        public static async Task<long> Add(Product product)
        {
            return await new ProductDB().Add(product);
        }

        // UPDATE
        public static async Task<bool> Update(ProductFull productFull)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.Update(productFull);

                // Update Product
                // Soft Delete
                await new ProductDB().UpdateStatusByPostId(productFull.PostId, 1, 0);

                productFull.Status = 1;

                Product product = new Product(productFull);
                if (product.Id == -1 || product.Id == 0)
                {
                    await Add(product);
                }
                else
                {
                    await Update(product);
                    await UpdateStatus(product.Id, 1);
                }

                scope.Complete();
                return true;
            }
        }

        public static async Task<bool> Accept(long postId, long productId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 1);
                bool productOk = await UpdateStatus(productId, 1);

                if (!postOk || !productOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long productId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 4);
                bool productOk = await UpdateStatus(productId, 4);

                if (!postOk || !productOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(Product product)
        {
            return await new ProductDB().Update(product);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new ProductDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            return await new ProductDB().UpdateStatusByPostId(postId, curStatus, newStatus);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new ProductDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                long productId = await new ProductDB().GetIdByPostId(postId);

                await new ProductReviewDB().DeleteByProductId(productId);
                await new ProductDB().DeleteByPostId(postId);

                scope.Complete();
            }
        }
    }
}