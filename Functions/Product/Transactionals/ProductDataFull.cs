using System.Collections.Generic;

namespace HeroServer
{
    public class ProductDataFull
    {
        public List<ProductFull> ProductFulls { get; set; }
        public List<ProductReviewFull> ProductReviewFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public ProductDataFull()
        {
        }

        public ProductDataFull(List<ProductFull> productFulls,
                               List<ProductReviewFull> productReviewFulls,
                               List<ContactFull> contactFulls,
                               List<LinkFull> linkFulls,
                               List<CommentFull> commentFulls)
        {
            ProductFulls = productFulls;
            ProductReviewFulls = productReviewFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
