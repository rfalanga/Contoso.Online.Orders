using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContosoOnlineOrders.Functions
{
    public class ContosoOnlineOrderingFunctionOpenAPIConfiguration : IOpenApiConfigurationOptions
    {
        public OpenApiInfo Info { get; set; } = new OpenApiInfo()
        {
            Version = "1.0.0",
            Title = "Notification APIs",
            Description = "Functions used by Contoso Online Ordering",
            Contact = new OpenApiContact()
            {
                Name = "Contoso",
                Email = "order-support@contoso.com"
            }
        };

        public List<OpenApiServer> Servers
        {
            get
            {
                return (new OpenApiServer[] { new OpenApiServer { Url = 
                    Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")
                }}).ToList();
            }
            set => throw new NotImplementedException();
        }
    }
}
