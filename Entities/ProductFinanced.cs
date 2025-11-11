using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class ProductFinanced
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CpiMin { get; set; }
        public int CpiMax { get; set; }
        public int CpiDefault { get; set; }
        public double AdvRate { get; set; }
        public double ReserveRate { get; set; }
        public int OverdueMax { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int ProductFinancedStatusId { get; set; }


        public ProductFinanced()
        {
        }

        public ProductFinanced(int id, int projectId, int cpiMin, int cpiMax, int cpiDefault, double advRate, double reserveRate, int overdueMax,
                               DateTime createDateTime, DateTime updateDateTime, int productFinancedStatusId)
        {
            Id = id;
            ProjectId = projectId;
            CpiMin = cpiMin;
            CpiMax = cpiMax;
            CpiDefault = cpiDefault;
            AdvRate = advRate;
            ReserveRate = reserveRate;
            OverdueMax = overdueMax;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            ProductFinancedStatusId = productFinancedStatusId;
        }
    }
}
