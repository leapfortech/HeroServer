using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterTreatmentRequest : RegisterPostRequest
    {
        public Treatment Treatment { get; set; }
        public List<Disease> Diseases { get; set; }

        public RegisterTreatmentRequest()
        {
        }

        public RegisterTreatmentRequest(Treatment treatment, List<Disease> diseases)
        {
            Treatment = treatment;
            Diseases = diseases;
        }
    }
}
