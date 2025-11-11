using System;

namespace HeroServer
{
    public class IveIncome
    {
        public int TypeId { get; set; }
        public double Amount { get; set; }
        public String Description { get; set; }

        public IveIncome()
        {
        }

        public IveIncome(int typeId, double amount, string description)
        {
            TypeId = typeId;
            Amount = amount;
            Description = description;
        }
    }
}
