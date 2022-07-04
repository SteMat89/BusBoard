using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BusBoard.Api
{
  public class BusAPI
  {
    private static RestClient ClientTfl = new RestClient("https://api.tfl.gov.uk");
    public static RestClient ClientPostalCode = new RestClient("http://api.postcodes.io/");
    public static void PrintBusesToConsoleLine(List<StopPoint> stopPoints)
    {
      foreach (var line in stopPoints)
      {
        Console.WriteLine(line.LineId + " => " + line.TimeToStation);
      }
    }
    public static List<StopPoint> DeserializeJsonStopPoints(string stringJson)
    {
      var stopPoints = JsonConvert.DeserializeObject<List<StopPoint>>(stringJson);
      return stopPoints;
    }
    public static string ConstructRequestForTheNextFiveBuses(string stationCode)
    {
      return $"StopPoint/{stationCode}/Arrivals";
    }
    public static List<StopPoint> GetNextFiveBuses(string stationCode)
    {
      List<StopPoint> nextFiveBuses = new List<StopPoint>();

      string requestString = ConstructRequestForTheNextFiveBuses(stationCode);
      
      var request = new RestRequest(requestString);
      
      var response = ClientTfl.Execute(request);
      
      string stringJson = response.Content;
      
      nextFiveBuses = DeserializeJsonStopPoints(stringJson);

      nextFiveBuses = nextFiveBuses.OrderBy(stop => stop.TimeToStation).Take(5).ToList();
      
      return nextFiveBuses;
    }
    
    public static Location GetLocation(string postcode)
    {
      string rawRequest = "postcodes/" + postcode;
      
      var request = new RestRequest(rawRequest);
      var response = ClientPostalCode.Execute(request);
      
      var des = JObject.Parse(response.Content);

      string lng = des["result"]["longitude"].ToString();
      string lat = des["result"]["latitude"].ToString();

      var location = new Location(lng, lat);
      
      return location;
    }

    public static string ConstructRequestToGetNearestStopPoints(Location location)
    {
      return $"StopPoint/?lat={location.Latitude}&lon={location.Longitude}&stopTypes=NaptanBusCoachStation,NaptanBusWayPoint," +
             "NaptanOnstreetBusCoachStopPair,NaptanPublicBusCoachTram";
    }

    public static void PrintStation(Station station)
    {
      Console.WriteLine("ID:\t\t" + station.Id+ "\n" +
                        "Distance\t" + station.Distance +"\n"+
                        "Common Name\t"+ station.CommonName);
    }
    
    public static List<Station> GetNearestStations(Location location)
    {
      var stopTypes = "NaptanBusCoachStation," +
                      "NaptanBusWayPoint," +
                      "NaptanOnstreetBusCoachStopPair," +
                      "NaptanPublicBusCoachTram";
      
      var request = new RestRequest("StopPoint");
      request.AddParameter("lat", location.Latitude);
      request.AddParameter("lon", location.Longitude);
      request.AddParameter("stopTypes", stopTypes);
      var response = ClientTfl.Execute(request);
      
      var result = JObject.Parse(response.Content);

      List<Station> nearestStations = new List<Station>();
      try
      {
        int at = 0;
        foreach (var var in result["stopPoints"])
        {
          string idStation = result["stopPoints"][at]["id"].ToString();
          var nextFiveBuses = GetNextFiveBuses(idStation);
          if (nextFiveBuses.Count != 0)
          {
            string distance  = result["stopPoints"][at]["distance"].ToString();
            string commonName = result["stopPoints"][at]["commonName"].ToString();
            
            nearestStations.Add(new Station(idStation, distance, commonName, nextFiveBuses));
            
            PrintStation(nearestStations[nearestStations.Count - 1]);
            PrintBusesToConsoleLine(nextFiveBuses);
            Console.WriteLine();
          }
          at++;
          if (nearestStations.Count == 2)
          {
            break;
          }
        }
      }
      catch (Exception ex)
      { 
        Debug.Write("Index not found.");
      }
      return nearestStations;
    }
  }
}
