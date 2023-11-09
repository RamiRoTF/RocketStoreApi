using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RocketStoreApi.Models;

namespace RocketStoreApi.PositionStack
{
    /// <summary>
    /// Class to Consume Position Stack API.
    /// </summary>
    public class RestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public Forward Item { get; private set; }

        private string apiKey = "f3768bc92a7a33fa246282abcef146e9";

        private string basepath = $"http://api.positionstack.com/v1/";

        /// <summary>
        /// Initializes a new instance of the <see cref="RestService"/> class.
        /// to initialize the HttpClient and JsonSerializerOptions.
        /// </summary>
        public RestService()
        {
            this._client = new HttpClient();
            this._serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        /// <summary>
        /// Method for performing a forward geocoding request.
        /// </summary>
        /// <param name="city"> The city of the customer.</param>
        /// <returns> return a Forward Object.</returns>
        public async Task<Forward> GetForwardAsync(string city)
        {
            this.Item = new Forward();
            string path = this.basepath + "forward?access_key=" + this.apiKey + "&query="+city;
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
                throw;
            }

            return this.Item;
        }
    }
}
