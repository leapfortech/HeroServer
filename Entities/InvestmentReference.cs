using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class InvestmentReference
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int InvestmentId { get; set; }
        public int ReferenceTypeId { get; set; }
        public String Name { get; set; }
        public int PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public String Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public InvestmentReference()
        {
        }

        public InvestmentReference(int id, int appUserId, int investmentId, int referenceTypeId, String name, int phoneCountryId, String phone, String email, String description, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            InvestmentId = investmentId;
            ReferenceTypeId = referenceTypeId;
            Name = name;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
            Description = description;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
