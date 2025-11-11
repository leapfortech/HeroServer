using System;

namespace HeroServer
{
    public class Identity
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String FirstName3 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String LastNameMarried { get; set; }
        public int GenderId { get; set; }
        public DateTime BirthDate { get; set; }
        public int BirthCountryId { get; set; }
        public int BirthStateId { get; set; }
        public int BirthCityId { get; set; }
        public String NationalityIds { get; set; }
        public int MaritalStatusId { get; set; }
        public String Occupation { get; set; }
        public String Nit { get; set; }
        public String DpiCui { get; set; }
        public DateTime DpiIssueDate { get; set; }
        public DateTime DpiDueDate { get; set; }
        public String DpiVersion { get; set; }
        public String DpiSerie { get; set; }
        public String DpiMrz { get; set; }
        public int PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public int IsPep { get; set; }
        public int HasPepIdentity { get; set; }
        public int IsCpe { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Identity()
        {
        }

        public Identity(int id, int appUserId, String firstName1, String firstName2, String firstName3, String lastName1, String lastName2, String lastNameMarried,
                        int genderId, DateTime birthDate, int birthCountryId, int birthStateId, int birthCityId, String nationalityIds, int maritalStatusId,
                        String occupation, String nit, String dpiCui, DateTime dpiIssueDate, DateTime dpiDueDate, String dpiVersion, String dpiSerie, String dpiMrz,
                        int phoneCountryId, String phone, String email, int isPep, int hasPepIdentity, int isCpe,
                        DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            FirstName3 = firstName3;
            LastName1 = lastName1;
            LastName2 = lastName2;
            LastNameMarried = lastNameMarried;
            GenderId = genderId;
            BirthDate = birthDate;
            BirthCountryId = birthCountryId;
            BirthStateId = birthStateId;
            BirthCityId = birthCityId;
            NationalityIds = nationalityIds;
            MaritalStatusId = maritalStatusId;
            Occupation = occupation;
            Nit = nit;
            DpiCui = dpiCui;
            DpiIssueDate = dpiIssueDate;
            DpiDueDate = dpiDueDate;
            DpiVersion = dpiVersion;
            DpiSerie = dpiSerie;
            DpiMrz = dpiMrz;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Email = email;
            IsPep = isPep;
            HasPepIdentity = hasPepIdentity;
            IsCpe = isCpe;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
