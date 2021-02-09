using ConsotoOnlineOrders.RefitClient;
using ContosoOnlineOrders.Abstractions.Models;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContosoMobileOrders.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        static readonly string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ?
                                            "http://10.0.2.2:5000" :
                                            "http://localhost:5000";


        public ObservableRangeCollection<Product> Products { get; set; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand AddCommand { get; }
        IContosoOnlineOrdersApiClient client;
        public ProductsViewModel()
        {
            client = RestService.For<IContosoOnlineOrdersApiClient>(BaseAddress);
            Products = new ObservableRangeCollection<Product>();
            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand(Add);
        }

        async Task Refresh()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var products = await client.GetProducts();
                Products.ReplaceRange(products);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task Add()
        {
            if (IsBusy)
                return;

            try
            {
                var name = await App.Current.MainPage.DisplayPromptAsync("New Product", "Enter product name:");
                if (string.IsNullOrWhiteSpace(name))
                    return;

                await client.CreateProduct(new CreateProductRequest
                {
                    Id = 1000 + Products.Count,
                    InventoryCount = 1,
                    Name = name
                });                
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
