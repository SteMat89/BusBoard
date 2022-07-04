using System.Collections.Generic;

namespace BusBoard.Api
{
    public class Station
    {
        public string Id { get; set; }
        public string Distance { get; set; }
        public string CommonName { get; set; }

        public List<StopPoint> NextFiveBuses { get; set; }

        public Station(string id, string distance, string commonName, List<StopPoint> nextFiveBuses)
        {
            this.Id = id;
            this.Distance = distance;
            this.CommonName = commonName;
            this.NextFiveBuses = nextFiveBuses;
        }
    }
}