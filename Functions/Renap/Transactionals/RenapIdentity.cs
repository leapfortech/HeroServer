using System;

namespace HeroServer
{
    public class RenapIdentity
    {
        public int Id { get; set; }
        public int AppUserId { get; set; } = -1;
        public String Cui { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String FirstName3 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public String LastNameMarried { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public String Gender { get; set; }
        public String MaritalStatus { get; set; }
        public String Nationality { get; set; }
        public String BirthCountry { get; set; }
        public String BirthState { get; set; }
        public String BirthCity { get; set; }
        public String CedulaResidence { get; set; }
        public String CedulaOrder { get; set; }
        public String CedulaRegister { get; set; }
        public String Occupation { get; set; }
        public DateTime? DpiDueDate { get; set; }
        public String DpiVersion { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int Status { get; set; }


        public RenapIdentity()
        {
        }

        public RenapIdentity(int id, int appUserId, String cui, String firstName1, String firstName2, String firstName3, String lastName1, String lastName2, String lastNameMarried,
                             DateTime birthDate, DateTime? deathDate, String gender, String maritalStatus, String nationality, String birthCountry, String birthState, String birthCity,
                             String cedulaResidence, String cedulaOrder, String cedulaRegister, String occupation, DateTime? dpiDueDate, String dpiVersion, DateTime createDateTime, int status)
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
            Gender = gender;
            MaritalStatus = maritalStatus;
            Nationality = nationality;
            BirthCountry = birthCountry;
            BirthState = birthState;
            BirthCity = birthCity;
            CedulaResidence = cedulaResidence;
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
