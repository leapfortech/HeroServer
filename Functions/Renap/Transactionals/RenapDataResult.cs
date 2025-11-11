using System;

namespace HeroServer
{
    public class RenapDataResult : LeapResult
    {
        public float? FirstName1 { get; set; }
        public float? FirstName2 { get; set; }
        public float? FirstName3 { get; set; }
        public float? LastName1 { get; set; }
        public float? LastName2 { get; set; }
        public float? LastNameMarried { get; set; }

        public float? Gender { get; set; }
        public float? MaritalStatus { get; set; }
        public float? Nationality { get; set; }

        public float? BirthDate { get; set; }
        public float? BirthCountry { get; set; }
        public float? BirthState { get; set; }
        public float? BirthCity { get; set; }
        public float? IsAlive { get; set; }

        public float? CedulaResidence { get; set; }
        public float? CedulaOrder { get; set; }
        public float? CedulaRegister { get; set; }

        public float? DpiDueDate { get; set; }
        public float? DpiVersion { get; set; }


        public RenapDataResult()
        {
        }

        public RenapDataResult(float? firstName1, float? firstName2, float? firstName3, float? lastName1, float? lastName2, float? lastNameMarried,
                               float? gender, float? maritalStatus, float? nationality, float? birthDate, float? birthCountry, float? birthState, float? birthCity, float? isAlive,
                               float? cedulaResidence, float? cedulaOrder, float? cedulaRegister, float? dpiDueDate, float? dpiVersion)
        {
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            FirstName3 = firstName3;
            LastName1 = lastName1;
            LastName2 = lastName2;
            LastNameMarried = lastNameMarried;

            Gender = gender;
            MaritalStatus = maritalStatus;
            Nationality = nationality;

            BirthDate = birthDate;
            BirthCountry = birthCountry;
            BirthState = birthState;
            BirthCity = birthCity;
            IsAlive = isAlive;

            CedulaResidence = cedulaResidence;
            CedulaOrder = cedulaOrder;
            CedulaRegister = cedulaRegister;

            DpiDueDate = dpiDueDate;
            DpiVersion = dpiVersion;
        }
    }
}
