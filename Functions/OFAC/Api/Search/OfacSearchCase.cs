using System;

namespace HeroServer
{
    public class OfacSearchCase
    {
#pragma warning disable IDE1006 // Naming Styles
        public String id { get; set; }
        public String name { get; set; }
        public String idNumber { get; set; }
        public String cryptoId { get; set; }
        public OfacAddressResult address { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacSearchCase()
        {

        }

        public OfacSearchCase(String id, String name, String idNumber, String cryptoId, OfacAddressResult address)
        {
            this.id = id;
            this.name = name;
            this.idNumber = idNumber;
            this.cryptoId = cryptoId;
            this.address = address;
        }

        public OfacSearchCase(OfacCase ofacCase)
        {
            id = null;
            name = ofacCase.Name;
            idNumber = (ofacCase.Identification == null || ofacCase.Identification.Length == 0) ? null : ofacCase.Identification[0].IdNumber;
            cryptoId = null;
            address = null;
        }
    }
}
