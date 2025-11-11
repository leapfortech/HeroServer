using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class ProjectInfo
    {
        public Project Project { get; set; }

        public Address Address { get; set; }
        public ProductFractionated ProductFractionated { get; set; }
        public ProductFinanced ProductFinanced { get; set; }
        public ProductPrepaid ProductPrepaid { get; set; }
        public List<CpiRange> CpiRanges { get; set; }
        public List<ProjectInformation> Informations { get; set; }
        public List<String> Images { get; set; }


        public ProjectInfo()
        {
        }

        public ProjectInfo(Project project, Address address, ProductFractionated productFractionated, ProductFinanced productFinanced, ProductPrepaid productPrepaid,
                           List<CpiRange> cpiRanges, List<ProjectInformation> informations, List<String> images)
        {
            Project = project;
            Address = address;
            ProductFractionated = productFractionated;
            ProductFinanced = productFinanced;
            ProductPrepaid = productPrepaid;
            CpiRanges = cpiRanges;
            Informations = informations;
            Images = images;
        }
    }
}
