using System;

namespace HeroServer
{
    public class OfacScreenCase
    {
#pragma warning disable IDE1006 // Naming Styles
        public String id { get; set; }
        public String name { get; set; }
        public String type { get; set; }
        public String dob { get; set; }
        public String gender { get; set; }
        public String citizenship { get; set; }
        public String nationality { get; set; }
        public String phoneNumber { get; set; }
        public String emailAddress { get; set; }
        public String cryptoId { get; set; }
        public OfacAddressResult address { get; set; }
        public OfacIdentificationRequest[] identification { get; set; }
        public String externalId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacScreenCase()
        {

        }

        public OfacScreenCase(String id, String name, String type, String dob, String gender, String citizenship, String nationality,
                              String phoneNumber, String emailAddress, String cryptoId, OfacAddressResult address, OfacIdentificationRequest[] identification)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.dob = dob;
            this.gender = gender;
            this.citizenship = citizenship;
            this.nationality = nationality;
            this.phoneNumber = phoneNumber;
            this.emailAddress = emailAddress;
            this.cryptoId = cryptoId;
            this.address = address;
            this.identification = identification;
        }

        public OfacScreenCase(OfacCase ofacCase)
        {
            id = null;
            name = ofacCase.Name;
            type = "person";
            dob = ofacCase.Dob;
            gender = ofacCase.Gender;
            citizenship = null;
            nationality = null;
            phoneNumber = ofacCase.PhoneNumber;
            emailAddress = ofacCase.EmailAddress;
            cryptoId = null;
            address = null;
            if (ofacCase.Identification != null)
            {
                identification = new OfacIdentificationRequest[ofacCase.Identification.Length];
                for (int i = 0; i < ofacCase.Identification.Length; i++)
                    identification[i] = new OfacIdentificationRequest(ofacCase.Identification[i]);
            }
            else
                identification = null;
        }
    }
}
