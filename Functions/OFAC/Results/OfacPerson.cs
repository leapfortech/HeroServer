using System;

namespace HeroServer
{
    public class OfacPerson
    {
        public String Source { get; set; }
        public String SourceId { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }
        public String Gender { get; set; }
        public String[] BirthDates { get; set; }

        public OfacPerson()
        {
        }

        public OfacPerson(String source, String sourceId, String firstName, String middleName, String lastName, String gender, String[] birthDates)
        {
            Source = source;
            SourceId = sourceId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Gender = gender;
            BirthDates = birthDates;
        }

        public OfacPerson(OfacSanction ofacSanction)
        {
            Source = ofacSanction.source;
            SourceId = ofacSanction.sourceId;
            FirstName = ofacSanction.personDetails.firstName;
            MiddleName = ofacSanction.personDetails.middleName;
            LastName = ofacSanction.personDetails.lastName;
            Gender = ofacSanction.personDetails.gender;
            BirthDates = ofacSanction.personDetails.birthDates;
        }
    }
}
