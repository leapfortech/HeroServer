using System;

namespace HeroServer
{
    public class Dpi
    {
        // DPI FRONT
        public String CUI { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String FirstName3 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String LastNameMarried { get; set; }
        public int GenderId { get; set; }
        public int MaritalStatusId { get; set; }
        public String Nationality { get; set; }
        public DateTime BirthDate { get; set; }
        public int BirthCountryId { get; set; }
        public DateTime IssueDate { get; set; }


        // DPI BACK
        public int BirthDepartmentId { get; set; }
        public int BirthTownshipId { get; set; }
        public DateTime DueDate { get; set; }
        public int ResidenceCountryId { get; set; }
        public int ResidenceDepartmentId { get; set; }
        public int ResidenceTownshipId { get; set; }
        public String MRZ { get; set; }

        public Dpi()
        {
        }

        public Dpi(String cui, String firstName1, String firstName2, String firstName3, String lastName1, String lastName2, String lastNameMarried, int genderId, int maritalStatusId,
                   String nationality, DateTime birthDate, int birthCountryId, DateTime issueDate, int birthDepartmentId, int birthTownshipId, DateTime dueDate,
                   int residenceCountryId, int residenceDepartmentId, int residenceTownshipId, String mrz)
        {
            CUI = cui;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            FirstName3 = firstName3;
            LastName1 = lastName1;
            LastName2 = lastName2;
            LastNameMarried = lastNameMarried;
            GenderId = genderId;
            MaritalStatusId = maritalStatusId;
            Nationality = nationality;
            BirthDate = birthDate;
            BirthCountryId = birthCountryId;
            IssueDate = issueDate;

            BirthDepartmentId = birthDepartmentId;
            BirthTownshipId = birthTownshipId;
            DueDate = dueDate;
            ResidenceCountryId = residenceCountryId;
            ResidenceDepartmentId = residenceDepartmentId;
            ResidenceTownshipId = residenceTownshipId;
            MRZ = mrz;
        }
    }
}
