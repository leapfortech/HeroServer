using System;

namespace HeroServer
{
    public class Product
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long OriginCountryId { get; set; }
        public long SaleCountryId { get; set; }
        public long SaleStateId { get; set; }
        public long CurrencyId { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public long ContactIdentityId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Product() { }

        public Product(long id, long postId, long originCountryId, long saleCountryId, long saleStateId, long currencyId,
                       double price, double discountPrice, long contactIdentityId, DateTime createDateTime,
                       DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            OriginCountryId = originCountryId;
            SaleCountryId = saleCountryId;
            SaleStateId = saleStateId;
            CurrencyId = currencyId;
            Price = price;
            DiscountPrice = discountPrice;
            ContactIdentityId = contactIdentityId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
