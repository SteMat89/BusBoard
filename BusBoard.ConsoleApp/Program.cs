using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using BusBoard.Api;
namespace BusBoard.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Please insert the station code:");
      string stationCode = Console.ReadLine();
      Console.WriteLine("Please insert the postal code:");
      string postalCode = Console.ReadLine();
      BusAPI.PrintBusesToConsoleLine(BusAPI.GetNextFiveBuses(stationCode));
      var location = BusAPI.GetLocation(postalCode);
      BusAPI.GetNearestStations(location);
    }
  }
}
