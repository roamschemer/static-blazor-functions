using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Data;

namespace Api {
    public static class TempDataFunction {
        [FunctionName("temp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var r = new Random(DateTime.Now.Millisecond);
            var tempData = new TempData() {
                DateTime = DateTime.Now,
                Temp = r.Next(0, 1000) * 0.1
            };

            return new OkObjectResult(tempData);
        }
    }
}
