using AlertApp.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using DevExpress.Maui.Core.Internal;
using System.Text;

namespace AlertApp.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        public MockDataStore()
        {
            HttpClientHandler insecureHandler = GetInsecureHandler();
            HttpClient = new(insecureHandler) { Timeout = new TimeSpan(0, 0, 10) };
            DateTime baseDate = DateTime.Today;
            this.items = new List<Item>() {
            new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description.", StartTime = baseDate.AddHours(1), EndTime = baseDate.AddHours(2), Value=17.098 },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description.", StartTime = baseDate.AddHours(2), EndTime = baseDate.AddHours(4), Value=9.985 },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description.", StartTime = baseDate.AddHours(3), EndTime = baseDate.AddHours(5), Value=9.597},
            new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description.", StartTime = baseDate.AddHours(5), EndTime = baseDate.AddHours(6), Value=9.834 },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description.", StartTime = baseDate.AddHours(9), EndTime = baseDate.AddHours(12), Value=3.287 },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description.", StartTime = baseDate.AddHours(12), EndTime = baseDate.AddHours(15), Value=81.2 }
        };
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            this.items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = this.items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            this.items.Remove(oldItem);
            this.items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = this.items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            this.items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(this.items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(this.items);
        }


        //API Service Mock
        private readonly HttpClient HttpClient;

        //public static readonly string ApiUrl = ON.Platform("http://10.0.2.2:5000/api/", "http://localhost:5000/api/");
        public static readonly string ApiUrl = ON.Platform("http://10.0.2.2:5000/api/", "http://localhost:5000/api/");

        private const string ApplicationJson = "application/json";

        public async Task<string> Authenticate(string userName, string password)
        {
#if DEBUG
            password = "";
#endif
            HttpResponseMessage tokenResponse;
            try
            {
                tokenResponse = await RequestTokenAsync(userName, password);
            }
            catch (Exception ex)
            {
#if DEBUG
                await Application.Current.MainPage.DisplayAlert("Couldn't reach the WebAPI service", ex.Message, "OK");
#endif
                return "An error occurred when processing the request";
            }
            if (tokenResponse.IsSuccessStatusCode)
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await tokenResponse.Content.ReadAsStringAsync());
                return null;
            }
            else
            {
                return await tokenResponse.Content.ReadAsStringAsync();
            }
        }

        private async Task<HttpResponseMessage> RequestTokenAsync(string userName, string password)
        {
            var data = new Dictionary<string, string> {
                { "userName", userName },
                { "password", password }
            };
            return await HttpClient.PostAsync($"{ApiUrl}Authentication/Authenticate",
                                                new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8,
                                                ApplicationJson));
        }

        // This method must be in a class in a platform project, even if
        // the HttpClient object is constructed in a shared project.
        private HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }
    }
}