using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;

namespace uju.function
{
      public class UjucontainerItem
    {
      [JsonProperty("id")]
      public string Id {get; set;}
      public string timestamp {get; set;}
      public double latitude {get; set;}
      public double longitude {get; set;}
      public double direction {get; set;}
      public string location {get; set;}
      public double sensorId {get; set;}
    }
      public class IotHubTrigger
      {
          private static HttpClient client = new HttpClient();
        
          [FunctionName("IoTHubTrigger")]
          public static void Run([IoTHubTrigger("messages/events", Connection = "AzureEventHubConnectionString")] EventData message,
          [CosmosDB(databaseName: "IoTData",
                                   collectionName: "ujucontainer",
                                   ConnectionStringSetting = "cosmosDBConnectionString")] out UjucontainerItem output,
                        ILogger log)
          {
              log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
 
        var jsonBody = Encoding.UTF8.GetString(message.Body);
        dynamic data = JsonConvert.DeserializeObject(jsonBody);
        string timestamp = data.latitude;
        double latitude = data.latitude;
        double longitude = data.longitude;
        double direction = data.direction;
        string location = data.location;
        double sensorId = data.sensorId;
      
        output = new UjucontainerItem
        {
          timestamp = timestamp,
          latitude = latitude,
          longitude = longitude,
          direction = direction,
          location = location,
          sensorId = sensorId
        };  
    }

      [FunctionName("Getujucontainer")]
      public static IActionResult Getujucontainer(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "position/")] HttpRequest req,
          [CosmosDB(databaseName: "IoTData", collectionName: "ujucontainer", ConnectionStringSetting = "cosmosDBConnectionString", SqlQuery = "SELECT * FROM c")] IEnumerable<UjucontainerItem> ujucontainerItems,
          ILogger log)
      {
          // Construct the HTML table
          StringBuilder htmlBuilder = new StringBuilder();
          htmlBuilder.Append("<table style='border-collapse: collapse; border: 1px solid black;'>");
          
          // Add the table headers
          htmlBuilder.Append("<tr>");
          htmlBuilder.Append("<th style='border: 1px solid black; padding: 5px;'>Timestamp</th>");
          htmlBuilder.Append("<th style='border: 1px solid black; padding: 5px;'>Latitude</th>");
          htmlBuilder.Append("<th style='border: 1px solid black; padding: 5px;'>Longitude</th>");
          htmlBuilder.Append("<th style='border: 1px solid black; padding: 5px;'>Direction</th>");
          htmlBuilder.Append("<th style='border: 1px solid black; padding: 5px;'>Location</th>");
          htmlBuilder.Append("<th style='border: 1px solid black; padding: 5px;'>Sensor ID</th>");
          htmlBuilder.Append("</tr>");

          // Add the table rows
          foreach (var item in ujucontainerItems)
          {
              htmlBuilder.Append("<tr>");
              htmlBuilder.AppendFormat("<td style='border: 1px solid black; padding: 5px;'>{0}</td>", item.timestamp);
              htmlBuilder.AppendFormat("<td style='border: 1px solid black; padding: 5px;'>{0}</td>", item.latitude);
              htmlBuilder.AppendFormat("<td style='border: 1px solid black; padding: 5px;'>{0}</td>", item.longitude);
              htmlBuilder.AppendFormat("<td style='border: 1px solid black; padding: 5px;'>{0}</td>", item.direction);
              htmlBuilder.AppendFormat("<td style='border: 1px solid black; padding: 5px;'>{0}</td>", item.location);
              htmlBuilder.AppendFormat("<td style='border: 1px solid black; padding: 5px;'>{0}</td>", item.sensorId);
              htmlBuilder.Append("</tr>");
          }
          
          // Close the HTML table
          htmlBuilder.Append("</table>");

          // Wrap the HTML content in a div element
          string combinedHtmlContent = "<div>" +htmlBuilder.ToString() + "</div>";

            return new ContentResult()
            {
              Content=combinedHtmlContent,
              ContentType="text/html",
              StatusCode=200
            };
      }
      }
}
