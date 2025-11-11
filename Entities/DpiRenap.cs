using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class DpiRenap
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public String Cui { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String FirstName3 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String LastNameMarried { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public int GenderId { get; set; }
        public int MaritalStatusId { get; set; }
        public int NationalityId { get; set; }
        public int BirthCountryId { get; set; }
        public int BirthStateId { get; set; }
        public int BirthCityId { get; set; }
        public String CedulaPlace { get; set; }
        public String CedulaOrder { get; set; }
        public String CedulaRegister { get; set; }
        public String Occupation { get; set; }
        public DateTime DpiDueDate { get; set; }
        public String DpiVersion { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int Status { get; set; }


        public DpiRenap()
        {
        }

        public DpiRenap(int id, int appUserId, String cui, String firstName1, String firstName2, String firstName3, String lastName1, String lastName2, String lastNameMarried, 
                        DateTime birthDate, DateTime? deathDate, int genderId, int maritalStatusId, int nationalityId, int birthCountryId, int birthStateId, 
                        int birthCityId, String cedulaPlace, String cedulaOrder, String cedulaRegister, String occupation, DateTime dpiDueDate, String dpiVersion,
                        DateTime createDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            Cui = cui;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            FirstName3 = firstName3;
            LastName1 = lastName1;
            LastName2 = lastName2;
            LastNameMarried = lastNameMarried;
            BirthDate = birthDate;
            DeathDate = deathDate;
            GenderId = genderId;
            MaritalStatusId = maritalStatusId;
            NationalityId = nationalityId;
            BirthCountryId = birthCountryId;
            BirthStateId = birthStateId;
            BirthCityId = birthCityId;
            CedulaPlace = cedulaPlace;
            CedulaOrder = cedulaOrder;
            CedulaRegister = cedulaRegister;
            Occupation = occupation;
            DpiDueDate = dpiDueDate;
            DpiVersion = dpiVersion;
            CreateDateTime = createDateTime;
            Status = status;
        }
    }
}
