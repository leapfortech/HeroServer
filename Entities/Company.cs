using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class Company
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String TradeName { get; set; }
        public int PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public int ContactPhoneCountryId { get; set; }
        public String ContactPhone { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String ZipCode { get; set; }
        public String Nit { get; set; }
        public String SatCode { get; set; }
        public String IvaAffiliation { get; set; }
        public String CofidiUser { get; set; }
        public String CofidiUserName { get; set; }
        public String FelLegend { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int CompanyStatusId { get; set; }


        public Company()
        {
        }

        public Company(int id, String name, String tradeName, int phoneCountryId, String phone, int contactPhoneCountryId, String contactPhone, int countryId, int stateId, int cityId, 
                       String address1, String address2, String zipCode, String nit, String satCode, String ivaAffiliation, String cofidiUser, String cofidiUserName, 
                       String felLegend, DateTime createDateTime, DateTime updateDateTime, int companyStatusId)
        {
            Id = id;
            Name = name;
            TradeName = tradeName;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            ContactPhoneCountryId = contactPhoneCountryId;
            ContactPhone = contactPhone;
            CountryId = countryId;
            StateId = stateId;
            CityId = cityId;
            Address1 = address1;
            Address2 = address2;
            ZipCode = zipCode;
            Nit = nit;
            SatCode = satCode;
            IvaAffiliation = ivaAffiliation;
            CofidiUser = cofidiUser;
            CofidiUserName = cofidiUserName;
            FelLegend = felLegend;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            CompanyStatusId = companyStatusId;
        }
    }
}
