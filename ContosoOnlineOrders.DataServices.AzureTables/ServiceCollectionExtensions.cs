using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.DataServices.AzureTables;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureTableStorageDataService(this IServiceCollection services, string connectionString)
        {
            var dataService = AzureTablesDataService.Create(connectionString);
            services.AddSingleton<IStoreDataService>(dataService);
            return services;
        }
    }
}
