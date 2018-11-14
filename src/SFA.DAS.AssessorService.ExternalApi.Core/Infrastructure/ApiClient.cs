namespace SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class ApiClient
    {
        protected internal readonly HttpClient _httpClient;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        protected ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region HTTP REST
        protected async Task<T> Get<T>(string uri)
        {
            using (var response = await _httpClient.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        protected async Task<U> Post<T, U>(string uri, T model)
        {
            string json = JsonConvert.SerializeObject(model, _jsonSettings);

            using (var response = await _httpClient.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(json, Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        protected async Task<U> Put<T, U>(string uri, T model)
        {
            string json = JsonConvert.SerializeObject(model, _jsonSettings);

            using (var response = await _httpClient.PutAsync(new Uri(uri, UriKind.Relative), new StringContent(json, Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        protected async Task<T> Delete<T>(string uri)
        {
            using (var response = await _httpClient.DeleteAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }
        #endregion
    }
}
