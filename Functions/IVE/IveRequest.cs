using System;

namespace HeroServer
{
    public class IveRequest
    {
        public Identity Identity { get; set; }
        
        public String Phone { get; set; }
        public String Email { get; set; }

        public Address Residence { get; set; }

        public IveIncome[] Incomes { get; set; }

        public IveRequest()
        {
        }

        public IveRequest(Identity identity, string phone, string email, Address residence, IveIncome[] incomes)
        {
            Identity = identity;
            Phone = phone;
            Email = email;
            Residence = residence;
            Incomes = incomes;
        }
    }
}
