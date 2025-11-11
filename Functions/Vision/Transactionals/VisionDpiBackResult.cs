using System;

namespace HeroServer
{
    public class VisionDpiBackResult : LeapResult
    {
        public String BirthCountry { get; set; }
        public String BirthState { get; set; }
        public String BirthCity { get; set; }

        public String MaritalStatus { get; set; }
        public String DueDate { get; set; }

        public String ResidenceCountry { get; set; }
        public String ResidenceState { get; set; }
        public String ResidenceCity { get; set; }

        public String Mrz { get; set; }

        public VisionDpiBackResult()
        {
        }

        public VisionDpiBackResult(String birthCountry, String birthDepartment, String birthTownship, String maritalStatus, DateTime? dueDate,
                                   String residenceCountry, String residenceState, String residenceCity, String mrz)
        {
            BirthCountry = birthCountry;
            BirthState = birthDepartment;
            BirthCity = birthTownship;

            MaritalStatus = maritalStatus;
            DueDate = $"{dueDate:dd/MM/yyyy}";

            ResidenceCountry = residenceCountry;
            ResidenceState = residenceState;
            ResidenceCity = residenceCity;

            Mrz = mrz;
        }
    }
}