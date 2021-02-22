using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using AdaptiveCards;
using AdaptiveCards.Templating;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using System.Linq;

namespace ContosoOnlineOrders.Functions
{
    public static class InventoryNotifier
    {
        [OpenApiOperation(operationId: nameof(SendLowStockNotification),
          Visibility = OpenApiVisibilityType.Important)
        ]
        [OpenApiRequestBody(contentType: "application/json",
          bodyType: typeof(Product[]),
          Required = true)
        ]
        [FunctionName(nameof(SendLowStockNotification))]
        public static async Task<IActionResult> SendLowStockNotification(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // variable for holding the json data during card-templating
            var json = "";

            // create the meta for the card
            var meta = new CardMetadata
            {
                Title = "Low Inventory Notification",
                Description = "The products below are low on inventory and should be re-stocked before customers are impacted.",
                CreatedUtc = DateTime.UtcNow,
                Creator = new Creator
                {
                    Name = "Inventory Robot",
                    ProfileImage = "https://github.com/dotnet/brand/blob/master/dotnet-bot-illustrations/dotnet-bot/dotnet-bot.png?raw=true"
                }
            };

            // open the card template
            using(Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ContosoOnlineOrders.Functions.InventoryNotificationCard.json"))
            {
                using (StreamReader rdr = new StreamReader(stream))
                {
                    json = rdr.ReadToEnd();
                }
            }

            // add all the products that need re-stocking to the card
            meta.Products = JsonConvert.DeserializeObject<List<Product>>(
                await req.ReadAsStringAsync()
            ).Where(_ => _.InventoryCount <= 5)
                .ToList();

            // render the card
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(json);
            json = template.Expand(meta);

            // return the result
            return new OkObjectResult(json);
        }
    }
}
