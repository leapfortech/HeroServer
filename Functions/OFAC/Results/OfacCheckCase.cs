using System;

namespace HeroServer
{
    public class OfacCheckCase
    {
        public String Name { get; set; }
        public OfacPerson[] Persons { get; set; }

        public OfacCheckCase()
        {

        }

        public OfacCheckCase(String name, OfacPerson[] persons)
        {
            Name = name;
            Persons = persons;
        }

        public OfacCheckCase(OfacScreenResult ofacScreenResult)
        {
            Name = ofacScreenResult.name;

            int nbPersons = ofacScreenResult.matchCount;
            Persons = new OfacPerson[nbPersons];
            for (int i = 0; i < nbPersons; i++)
                Persons[i] = new OfacPerson(ofacScreenResult.matches[i].sanction);
        }
    }
}
