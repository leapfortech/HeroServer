
namespace HeroServer
{
    public class ProjectProductFull
    {
        public ProjectFull ProjectFull { get; set; }
        public ProductFractionated ProductFractionated { get; set; }
        public ProductPrepaid ProductPrepaid { get; set; }
        public ProductFinanced ProductFinanced { get; set; }

        public ProjectProductFull()
        {
        }

        public ProjectProductFull(ProjectFull projectFull, ProductFractionated productFractionated,
                                  ProductPrepaid productPrepaid, ProductFinanced productFinanced)
        {
            ProjectFull = projectFull;
            ProductFractionated = productFractionated;
            ProductPrepaid = productPrepaid;
            ProductFinanced = productFinanced;
        }
    }
}
