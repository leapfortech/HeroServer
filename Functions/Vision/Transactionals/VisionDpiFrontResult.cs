using System;
using System.Text.Json.Serialization;

namespace HeroServer
{
    public class VisionDpiFrontResult : LeapResult
    {
        public String Cui { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String FirstName3 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String LastNameMarried { get; set; }

        public String Gender { get; set; }
        public String MaritalStatus { get; set; }
        public String Nationality { get; set; }
        public String BirthDate { get; set; }
        public String BirthCountry { get; set; }
        public String IssueDate { get; set; }
        public String IdentificationCountry { get; set; }
        public String Face { get; set; }

        public VisionDpiFrontResult()
        {
        }

        public VisionDpiFrontResult(String cui, String firstName1, String firstName2, String firstName3, String lastName1, String lastName2, String lastNameMarried,
                                    String gender, String maritalStatus, String nationality, DateTime? birthDate, String birthCountry, DateTime? issueDate, String identificationCountry, byte[] face)
        {
            Cui = cui;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            FirstName3 = firstName3;
            LastName1 = lastName1;
            LastName2 = lastName2;

            Gender = gender;
            MaritalStatus = maritalStatus;
            LastNameMarried = lastNameMarried;
            Nationality = nationality;
            BirthDate = $"{birthDate:dd/MM/yyyy}";
            BirthCountry = birthCountry;
            IssueDate = $"{issueDate:dd/MM/yyyy}";
            IdentificationCountry = identificationCountry;

            Face = face == null ? null : Convert.ToBase64String(face);
        }
    }
}