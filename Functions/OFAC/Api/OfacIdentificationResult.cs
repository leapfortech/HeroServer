using System;

namespace HeroServer
{
    public class OfacIdentificationResult
    {
#pragma warning disable IDE1006 // Naming Styles
        public String id { get; set; }
        public String type { get; set; }
        public String idNumber { get; set; }
        public String country { get; set; }
        public String issueDate { get; set; }
        public String expirationDate { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacIdentificationResult()
        {
        }

        public OfacIdentificationResult(String id, String type, String idNumber, String country, String issueDate, String expirationDate)
        {
            this.id = id;
            this.type = type;
            this.idNumber = idNumber;
            this.country = country;
            this.issueDate = issueDate;
            this.expirationDate = expirationDate;
        }
    }
}
