using System;

namespace HeroServer
{
    public class Product
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long ProductSubtypeId { get; set; }
        public long SaleCountryId { get; set; }
        public long SaleStateId { get; set; }
        public long CurrencyId { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public long DeliveryTypeId { get; set; }
        public String Annotation { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Product() { }

        public Product(long id, long postId, long productSubtypeId, long saleCountryId, long saleStateId, long currencyId,
                       double price, double discountPrice, long deliveryTypeId, String annotation, DateTime createDateTime,
                       DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            ProductSubtypeId = productSubtypeId;
            SaleCountryId = saleCountryId;
            SaleStateId = saleStateId;
            CurrencyId = currencyId;
            Price = price;
            DiscountPrice = discountPrice;
            DeliveryTypeId = deliveryTypeId;
            Annotation = annotation;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
