using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Data;
using System.IO;
using Newtonsoft.Json;

namespace Api {
    public static class TempDataFunction {
        [FunctionName("temp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "kunsei-iot-db",
                collectionName: "kunsei-iot-container",
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            string temp = req.Query["temp"];

            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body)) {
                requestBody = await streamReader.ReadToEndAsync();
            }
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            temp = temp ?? data?.temp;

            var r = new Random(DateTime.Now.Millisecond);
            var tempData = new TempData() {
                Name = name,
                DateTime = DateTime.Now,
                Temp = temp != null ? double.Parse(temp) : 0
            };

            if (!string.IsNullOrEmpty(name)) {
                await documentsOut.AddAsync(new {
                    id = System.Guid.NewGuid().ToString(),
                    name = tempData.Name,
                    temp = tempData.Temp,
                    datetime = tempData.DateTime,
                });
            }

            return new OkObjectResult(tempData);
        }
    }
}
