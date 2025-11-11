using System;

namespace HeroServer
{
    public class OfacPersonDetails
    {
#pragma warning disable IDE1006 // Naming Styles
        public String firstName { get; set; }
        public String middleName { get; set; }
        public String lastName { get; set; }
        public String title { get; set; }
        public String gender { get; set; }
        public String[] birthDates { get; set; }
        public String[] citizenships { get; set; }
        public String[] nationalities { get; set; }
        public String[] positions { get; set; }
        public String[] education { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public OfacPersonDetails()
        {
        }

        public OfacPersonDetails(String firstName, String middleName, String lastName, String title, String gender, String[] birthDates,
                                 String[] citizenships, String[] nationalities, String[] positions, String[] education)
        {
            this.firstName = firstName;
            this.middleName = middleName;
            this.lastName = lastName;
            this.title = title;
            this.gender = gender;
            this.birthDates = birthDates;
            this.citizenships = citizenships;
            this.nationalities = nationalities;
            this.positions = positions;
            this.education = education;
        }
    }
}
