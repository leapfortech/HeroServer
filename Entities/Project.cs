using System;

namespace HeroServer
{
    public class Project
    {
        public int Id { get; set; }
        public int ProjectTypeId { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Details { get; set; }
        public int ImageCount { get; set; }
        public double TotalArea { get; set; }
        public double TotalBuiltArea { get; set; }
        public int LevelCount { get; set; }
        public int CurrencyId { get; set; } = 47;
        public double CpiValue { get; set; }
        public int CpiTotal { get; set; }
        public int CpiCount { get; set; }
        public double TotalValue { get; set; }
        public DateTime StartDate { get; set; }
        public int DevelopmentTerm { get; set; }
        public double RentalGrowthRate { get; set; }
        public double CapitalGrowthRate { get; set; }
        public int EarningPeriodId { get; set; }
        public double ManagementCost { get; set; }
        public int Status { get; set; }


        public Project()
        {
        }

        public Project(int id, int projectTypeId, String code, String name, String description, String details, int imageCount,
                       double totalArea, double totalBuiltArea, int levelCount, int currencyId,
                       double cpiValue, int cpiTotal, int cpiCount, double totalValue, DateTime startDate, int developmentTerm,
                       double rentalGrowthRate, double capitalGrowthRate, int earningPeriodId, double managementCost, int status)
        {
            Id = id;
            ProjectTypeId = projectTypeId;
            Code = code;
            Name = name;
            Description = description;
            Details = details;
            ImageCount = imageCount;
            TotalArea = totalArea;
            TotalBuiltArea = totalBuiltArea;
            LevelCount = levelCount;
            CurrencyId = currencyId;
            CpiValue = cpiValue;
            CpiTotal = cpiTotal;
            CpiCount = cpiCount;
            TotalValue = totalValue;
            StartDate = startDate;
            DevelopmentTerm = developmentTerm;
            RentalGrowthRate = rentalGrowthRate;
            CapitalGrowthRate = capitalGrowthRate;
            EarningPeriodId = earningPeriodId;
            ManagementCost = managementCost;
            Status = status;
        }
    }
}
