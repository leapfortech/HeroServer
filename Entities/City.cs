using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class City
    {
        public int Id { get; set; }
        public int StateId { get; set; }
        public String Name { get; set; }
        public String Timezone { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public String Zips { get; set; }
        public int Status { get; set; }


        public City()
        {
        }

        public City(int id, int stateId, String name, String timezone, float? latitude, float? longitude, String zips, int status)
        {
            Id = id;
            StateId = stateId;
            Name = name;
            Timezone = timezone;
            Latitude = latitude;
            Longitude = longitude;
            Zips = zips;
            Status = status;
        }
    }
}
