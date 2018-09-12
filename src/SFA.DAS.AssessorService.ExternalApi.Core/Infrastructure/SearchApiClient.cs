namespace SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Search;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class SearchApiClient : ApiClient
    {
        public SearchApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<SearchResult>> Search(long uln, string lastname, int? standardCode = null)
        {
            return await Get<List<SearchResult>>($"search/{uln}/{lastname}/{standardCode}");
        }
    }
}
