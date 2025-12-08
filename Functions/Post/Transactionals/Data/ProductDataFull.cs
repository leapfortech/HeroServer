using System.Collections.Generic;

namespace HeroServer
{
    public class ProductDataFull
    {
        public List<ProductFull> ProductFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<ProductReviewFull> ProductReviewFulls { get; set; }

        public ProductDataFull()
        {
        }

        public ProductDataFull(List<ProductFull> productFulls,
                               List<ContactFull> contactFulls,
                               List<ProductReviewFull> productReviewFulls)
        {
            ProductFulls = productFulls;
            ContactFulls = contactFulls;
            ProductReviewFulls = productReviewFulls;
        }
    }
}
