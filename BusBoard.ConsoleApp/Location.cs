using System;

namespace BusBoard.ConsoleApp
{
    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Location(string lng, string lat)
        {
            Longitude = Convert.ToDouble(lng);
            Latitude = Convert.ToDouble(lat);
        }
    }
}