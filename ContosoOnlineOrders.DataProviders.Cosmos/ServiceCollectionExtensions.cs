using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.DataProviders.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosDbStorage(this IServiceCollection services, string connectionString)
        {
            services.AddCosmosRepository(
                options =>
                {
                    options.CosmosConnectionString = connectionString;
                    options.ContainerId = "Contoso";
                    options.DatabaseId = "OnlineOrders";
                    options.ContainerPerItemType = true;
                });

            services.AddSingleton<IStoreDataService, CosmosDataProvider>();
            return services;
        }
    }
}
