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

        public static async Task<ProductFull> GetFullById(long id, long likeAppUserId)
        {
            ProductFull productFull = await new ProductDB().GetFullById(id, likeAppUserId);

            if (productFull == null)
                return null;

            productFull.Images = await PostFunctions.GetImagesById(productFull.PostId, true);

            return productFull;
        }

        public static async Task<ProductFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            ProductFull productFull = await new ProductDB().GetFullByPostId(postId, likeAppUserId);

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

        // REGISTER
        public static async Task<long> Register(RegisterProductRequest registerProductRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerProductRequest.Post.PostTypeId = (long)PostType.Product;
                registerProductRequest.Product.PostId = await PostFunctions.Register(registerProductRequest);

                registerProductRequest.Product.Status = 1;
                id = await Add(registerProductRequest.Product);

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
        public static async Task<bool> Update(RegisterProductRequest registerProductRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerProductRequest);

                // Update Product
                // Soft Delete
                await new ProductDB().UpdateStatusByPostId(registerProductRequest.Post.Id, 1, 0);

                registerProductRequest.Product.PostId = registerProductRequest.Post.Id;
                registerProductRequest.Product.Status = 1;

                if (registerProductRequest.Product.Id == -1 || registerProductRequest.Product.Id == 0)
                {
                    await Add(registerProductRequest.Product);
                }
                else
                {
                    await Update(registerProductRequest.Product);
                    await UpdateStatus(registerProductRequest.Product.Id, 1);
                }

                scope.Complete();
                return true;
            }
        }

        public static async Task<bool> Accept(long postId, long productId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 3);
                bool productOk = await UpdateStatus(productId, 3);

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
                bool postOk = await PostFunctions.UpdateStatus(postId, 0);
                bool productOk = await UpdateStatus(productId, 0);

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