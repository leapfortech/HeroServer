
using System.Collections.Generic;

namespace HeroServer
{
    public class ProjectProductDataFull
    {
        public List<ProjectFull> ProjectFulls { get; set; }

        public List<CpiRange> CpiRanges { get; set; }
        public List<AddressFull> AddressFulls { get; set; }
        public List<ProjectInformation> ProjectInformations { get; set; }
        public List<OperationResult> OperationResults { get; set; }

        public List<ProductFractionated> ProductFractionateds { get; set; }
        public List<ProductFinanced> ProductFinanceds { get; set; }
        public List<ProductPrepaid> ProductPrepaids { get; set; }

        public ProjectProductDataFull()
        {
        }

        public ProjectProductDataFull(List<ProjectFull> projectFulls,

                                      List<CpiRange> cpiRanges,
                                      List<AddressFull> addressFulls,
                                      List<ProjectInformation> projectInformations,
                                      List<OperationResult> operationResults,

                                      List<ProductFractionated> productFractionateds,
                                      List<ProductFinanced> productFinanceds,
                                      List<ProductPrepaid> productPrepaids)
        {
            ProjectFulls = projectFulls;

            CpiRanges = cpiRanges;
            AddressFulls = addressFulls;
            ProjectInformations = projectInformations;
            OperationResults = operationResults;

            ProductFractionateds = productFractionateds;
            ProductFinanceds = productFinanceds;
            ProductPrepaids = productPrepaids;
        }
    }
}
