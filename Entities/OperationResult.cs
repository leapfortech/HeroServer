using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class OperationResult
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public double RevenueAmount { get; set; }
        public double CostAmount { get; set; }
        public DateTime CreateDateTime { get; set; }


        public OperationResult()
        {
        }

        public OperationResult(int id, int projectId, double revenueAmount, double costAmount, DateTime createDateTime)
        {
            Id = id;
            ProjectId = projectId;
            RevenueAmount = revenueAmount;
            CostAmount = costAmount;
            CreateDateTime = createDateTime;
        }
    }
}
