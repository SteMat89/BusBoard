using System.Security.Authentication.ExtendedProtection;
using BusBoard.ConsoleApp.Properties;

namespace BusBoard.ConsoleApp
{
    public class StopPoint
    {
        public string Id { get; set; }
        public int OperationType { get; set; }
        public string VehicleId { get; set; }
        public string NaptanId { get; set; }
        public string StationName { get; set; }
        public string LineId { get; set; }
        public string PlatformName { get; set; }
        public string Direction { get; set; }
        public string Bearing { get; set; }
        public string DestinationNaptainId { get; set; }
        public string DestinationName { get; set; }
        public string TimeStamp { get; set; }
        public int TimeToStation { get; set; }
        public string CurrentLocation { get; set; }
        public string Towards { get; set; }
        public string ExpectedArrival { get; set; }
        public string TimeToLive { get; set; }
        public string ModelName { get; set; }
        public Timing Timing { get; set; }

    }
}