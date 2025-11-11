using System;

namespace HeroServer
{
    public class ProductFractionated
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CpiMin { get; set; }
        public int CpiMax { get; set; }
        public int CpiDefault { get; set; }
        public double ReserveRate { get; set; }
        public int OverdueMax { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int ProductFractionatedStatusId { get; set; }


        public ProductFractionated()
        {
        }

        public ProductFractionated(int id, int projectId, int cpiMin, int cpiMax, int cpiDefault, double reserveRate, int overdueMax,
                                   DateTime createDateTime, DateTime updateDateTime, int productFractionatedStatusId)
        {
            Id = id;
            ProjectId = projectId;
            CpiMin = cpiMin;
            CpiMax = cpiMax;
            CpiDefault = cpiDefault;
            ReserveRate = reserveRate;
            OverdueMax = overdueMax;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            ProductFractionatedStatusId = productFractionatedStatusId;
        }
    }
}
