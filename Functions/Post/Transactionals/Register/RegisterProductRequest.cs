using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterProductRequest : RegisterPostRequest
    {
        public Product Product { get; set; }
        public Contact Contact { get; set; }
        public List<ProductReview> ProductReviews { get; set; }

        public RegisterProductRequest()
        {
        }

        public RegisterProductRequest(Product product, Contact contact, List<ProductReview> productReviews)
        {
            Product = product;
            Contact = contact;
            ProductReviews = productReviews;
        }
    }
}
