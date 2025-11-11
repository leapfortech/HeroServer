using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class ProjectFull
    {
        public int ProjectId { get; set; }
        public String ProjectType { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Details { get; set; }
        public int ImageCount { get; set; }
        public double TotalArea { get; set; }
        public double TotalBuiltArea { get; set; }
        public int LevelCount { get; set; }
        public String Currency { get; set; }
        public String CurrencySymbol { get; set; }
        public double CpiValue { get; set; }
        public int CpiTotal { get; set; }
        public int CpiCount { get; set; }
        public double TotalValue { get; set; }
        public DateTime StartDate { get; set; }
        public int DevelopmentTerm { get; set; }
        public double RentalGrowthRate { get; set; }
        public double CapitalGrowthRate { get; set; }
        public String EarningPeriod { get; set; }
        public double ManagementCost { get; set; }
        public List<CpiRange> CpiRanges { get; set; }
        public AddressFull AddressFull { get; set; }
        public List<ProjectInformation> Informations { get; set; }
        public List<OperationResult> OperationResults { get; set; }
        public String CoverImage { get; set; }
        public int Status { get; set; }


        public ProjectFull()
        {
        }

        public ProjectFull(int projectId, String projectType, String code, String name, String description, String details, int imageCount,
                           double totalArea, double totalBuiltArea, int levelCount, String currency, String currencySymbol,
                           double cpiValue, int cpiTotal, int cpiCount, double totalValue, DateTime startDate, int developmentTerm,
                           double rentalGrowthRate, double capitalGrowthRate, String earningPeriod, double managementCost,
                           List<CpiRange> cpiRanges, AddressFull addressFull, List<ProjectInformation> informations,
                           List<OperationResult> operationResults, String coverImage, int status)
        {
            ProjectId = projectId;
            ProjectType = projectType;
            Code = code;
            Name = name;
            Description = description;
            Details = details;
            ImageCount = imageCount;
            TotalArea = totalArea;
            TotalBuiltArea = totalBuiltArea;
            LevelCount = levelCount;
            Currency = currency;
            CurrencySymbol = currencySymbol;
            CpiValue = cpiValue;
            CpiTotal = cpiTotal;
            CpiCount = cpiCount;
            TotalValue = totalValue;
            StartDate = startDate;
            DevelopmentTerm = developmentTerm;
            RentalGrowthRate = rentalGrowthRate;
            CapitalGrowthRate = capitalGrowthRate;
            EarningPeriod = earningPeriod;
            ManagementCost = managementCost;

            CpiRanges = cpiRanges;
            AddressFull = addressFull;
            Informations = informations;
            OperationResults = operationResults;

            CoverImage = coverImage;
            Status = status;
        }
    }
}
