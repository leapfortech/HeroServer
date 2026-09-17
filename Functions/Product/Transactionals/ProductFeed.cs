using System;

namespace HeroServer
{
    public class ProductFeed
    {
        public String TitleImage { get; set; }
        public String Title { get; set; }
        public String ProductSubtype { get; set; }
        public String SaleCountry { get; set; }
        public String SaleState { get; set; }
        public String Currency { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public String Link { get; set; }

        public ProductFeed()
        {

        }

        public ProductFeed(String titleImage, String title, String productSubtype, String saleCountry, String saleState, String currency, double price, double discountPrice, String link)
        {
            TitleImage = titleImage;
            Title = title;
            ProductSubtype = productSubtype;
            SaleCountry = saleCountry;
            SaleState = saleState;
            Currency = currency;
            Price = price;
            DiscountPrice = discountPrice;
            Link = link;
        }
    }
}
