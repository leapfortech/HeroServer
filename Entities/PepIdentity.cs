using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PepIdentity
    {
        public int Id { get; set; }
        public int IdentityId { get; set; }
        public int RelationshipTypeId { get; set; }
        public String RelatioshipTypeDescription { get; set; }
        public int PartnershipMotiveId { get; set; }
        public String PartnershipMotiveDescription { get; set; }
        public String InstitutionName { get; set; }
        public int InstitutionCountryId { get; set; }
        public String JobTitle { get; set; }
        public int IsForeigner { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDateTime { get; set; }


        public PepIdentity()
        {
        }

        public PepIdentity(int id, int identityId, int relationshipTypeId, String relatioshipTypeDescription, int partnershipMotiveId, String partnershipMotiveDescription, String institutionName, int institutionCountryId, String jobTitle, int isForeigner, DateTime createDatetime, DateTime updateDateTime)
        {
            Id = id;
            IdentityId = identityId;
            RelationshipTypeId = relationshipTypeId;
            RelatioshipTypeDescription = relatioshipTypeDescription;
            PartnershipMotiveId = partnershipMotiveId;
            PartnershipMotiveDescription = partnershipMotiveDescription;
            InstitutionName = institutionName;
            InstitutionCountryId = institutionCountryId;
            JobTitle = jobTitle;
            IsForeigner = isForeigner;
            CreateDatetime = createDatetime;
            UpdateDateTime = updateDateTime;
        }
    }
}
