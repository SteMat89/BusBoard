using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using JsonSerializer = RestSharp.Serialization.Json.JsonSerializer;

namespace BusBoard.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      var client = new RestClient("https://api.tfl.gov.uk");
      string stationCde, requestString = "StopPoint/";
      List<StopPoint> stopPoints = new List<StopPoint>();

      Console.WriteLine("Please insert your station's Id:");
      stationCde=  Console.ReadLine();
      
      requestString += stationCde;
      requestString += "/Arrivals";
      
      var request = new RestRequest(requestString);
      var response = client.Execute(request);
      
      string stringJson = response.Content;
      
      var run = new Program();
      stopPoints = run.DeserializeJsonStopPoints(stringJson);

      stopPoints = stopPoints.OrderBy(stop => stop.TimeToStation).Take(5).ToList();

      foreach (var line in stopPoints)
      {
        Console.WriteLine(line.LineId + " => " + line.TimeToStation);
      }
    }

    public List<StopPoint> DeserializeJsonStopPoints(string stringJson)
    {
      var stopPoints = JsonConvert.DeserializeObject<List<StopPoint>>(stringJson);
      return stopPoints;
    }
    
  }
}
