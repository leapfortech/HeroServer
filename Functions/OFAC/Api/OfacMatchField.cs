using System;

namespace HeroServer
{
    public class OfacMatchField
    {
#pragma warning disable IDE1006 // Naming Styles
        public String similarity { get; set; }
        public String fieldName { get; set; }
        public String caseField { get; set; }
        public String sanctionField { get; set; }
        public String sanctionFieldNote { get; set; }
        public String sanctionFieldId { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacMatchField()
        {
        }

        public OfacMatchField(String similarity, String fieldName, String caseField, String sanctionField, String sanctionFieldNote, String sanctionFieldId)
        {
            this.similarity = similarity;
            this.fieldName = fieldName;
            this.caseField = caseField;
            this.sanctionField = sanctionField;
            this.sanctionFieldNote = sanctionFieldNote;
            this.sanctionFieldId = sanctionFieldId;
        }
    }
}
