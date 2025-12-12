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

        public static async Task<ProductFull> GetFullById(long id)
        {
            return await new ProductDB().GetFullById(id);
        }

        public static async Task<ProductFull> GetFullByPostId(long postId)
        {
            return await new ProductDB().GetFullByPostId(postId);
        }

        public static async Task<List<ProductFull>> GetFullsByStatus(int status)
        {
            ProductDataFull productDataFull = await new ProductDB().GetDataFullByStatus(status);

            return GetFulls(productDataFull);
        }

        public static List<ProductFull> GetFulls(ProductDataFull productDataFull)
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

                productFulls.Add(productFull);
            }

            return productFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterProductRequest registerProductRequest)
        {
            long productId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerProductRequest.Product.Status = 1;

                productId = await new ProductDB().Add(registerProductRequest.Product);

                for (int i = 0; i < registerProductRequest.ProductReviews.Count; i++)
                {
                    registerProductRequest.ProductReviews[i].ProductId = productId;
                    registerProductRequest.ProductReviews[i].Status = 1;

                    await new ProductReviewDB().Add(registerProductRequest.ProductReviews[i]);
                }

                scope.Complete();
            }

            return productId;
        }

        // ADD
        public static async Task<long> Add(Product product)
        {
            return await new ProductDB().Add(product);
        }

        // UPDATE
        public static async Task<bool> Update(Product product)
        {
            return await new ProductDB().Update(product);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new ProductDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new ProductDB().DeleteById(id);
        }
    }
}