using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterProductRequest : RegisterPostRequest
    {
        public Product Product { get; set; }

        public RegisterProductRequest()
        {
        }

        public RegisterProductRequest(Product product)
        {
            Product = product;
        }
    }
}
