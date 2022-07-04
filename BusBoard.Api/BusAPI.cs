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

    public static List<StopPoint> GetNextFiveBuses(string stationCode)
    {
      var client = new RestClient("https://api.tfl.gov.uk");
      string  requestString = "StopPoint/";
      List<StopPoint> nextFiveBuses = new List<StopPoint>();
      
      requestString += stationCode;
      requestString += "/Arrivals";
      
      var request = new RestRequest(requestString);
      var response = client.Execute(request);
      
      string stringJson = response.Content;
      
      nextFiveBuses = DeserializeJsonStopPoints(stringJson);

      nextFiveBuses = nextFiveBuses.OrderBy(stop => stop.TimeToStation).Take(5).ToList();
      
      return nextFiveBuses;
    }
    
    public static Location GetLocation(string postcode)
    {
      var client = new RestClient("https://api.postcodes.io");
      string rawRequest = "postcodes/" + postcode;
      
      var request = new RestRequest(rawRequest);
      var response = client.Execute(request);
      
      var des = JObject.Parse(response.Content);

      string lng = des["result"]["longitude"].ToString();
      string lat = des["result"]["latitude"].ToString();

      var location = new Location(lng, lat);
      
      return location;
    }

    public static string ConstructRequestToGetNearestStopPoints(Location location)
    {
      string requestString = "StopPoint/";
      StringBuilder sb = new StringBuilder(requestString);
      sb.Append("?lat=");
      sb.Append(location.Latitude);
      sb.Append("&lon=");
      sb.Append(location.Longitude);
      sb.Append("&stopTypes=NaptanBusCoachStation,NaptanBusWayPoint," +
                "NaptanOnstreetBusCoachStopPair,NaptanPublicBusCoachTram");
      return sb.ToString();
    }

    public static void PrintStation(Station station)
    {
      Console.WriteLine("ID:\t\t" + station.Id+ "\n" +
                        "Distance\t" + station.Distance +"\n"+
                        "Common Name\t"+ station.CommonName);
    }
    
    public static List<Station> GetNearestStopPoint(Location location)
    {
      var client = new RestClient("https://api.tfl.gov.uk");
      
      string rawRequest = ConstructRequestToGetNearestStopPoints(location);
      var request = new RestRequest(rawRequest);
      
      var response = client.Execute(request);
      
      var result = JObject.Parse(response.Content);

      List<Station> nearestStations = new List<Station>();
      try
      {
        int at = 0;
        while (nearestStations.Count < 2)
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
