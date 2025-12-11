using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterProductRequest : RegisterPostRequest
    {
        public Product Product { get; set; }
        public List<ProductReview> ProductReviews { get; set; }

        public RegisterProductRequest()
        {
        }

        public RegisterProductRequest(Product product, List<ProductReview> productReviews)
        {
            Product = product;
            ProductReviews = productReviews;
        }
    }
}
