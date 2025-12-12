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