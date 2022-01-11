using SharedModels.Models;
using System.Net.Http.Json;
namespace Blazor_ClientApp.ClientCaller
{
    public class OrdersProxy
    {
        private HttpClient _httpClient;
        private string url;

        public OrdersProxy()
        {
            _httpClient = new HttpClient();
            url = "https://localhost:7145/";
        }

        public async Task<List<Orders>> GetOrdersAsync()
        {
            return null;
        }

        public async Task<Orders> GetOrderAsync(int id)
        {
            return null;
        }


        public async Task<Orders> PostOrderAsync(Orders order)
        {
            return null;
        }

        public async Task<Orders> PutOrderAsync(int id,Orders order)
        {
            return null;
        }
        public async Task<Orders> ApproveOrderAsync(int id, Orders order)
        {
            return null;
        }
        public async Task<Orders> DeleteOrderAsync(int id)
        {
            return null;
        }
    }
}
