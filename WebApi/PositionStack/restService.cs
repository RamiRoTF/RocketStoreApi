using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RocketStoreApi.Models;

namespace RocketStoreApi.PositionStack
{
    public class RestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public Forward Item { get; private set; }

        private string apiKey = "212b66992ff71418760fa8f885d30bc4";

        private string basepath = $"https://api.positionstack.com/v1/";

        public RestService()
        {
            this._client = new HttpClient();
            this._serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<Forward> GetForwardAsync()
        {
            this.Item = new Forward();
            string path = this.basepath + this.apiKey + "&query=Braga";
            Uri uri = new Uri(path);
            try
            {
                HttpResponseMessage response = await this._client.GetAsync(uri).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    this.Item = JsonSerializer.Deserialize<Forward>(content, this._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }

            return this.Item;
        }
    }
}
