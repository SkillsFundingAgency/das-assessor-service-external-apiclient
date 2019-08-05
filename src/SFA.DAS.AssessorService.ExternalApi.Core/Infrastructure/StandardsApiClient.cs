namespace SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Standards;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class StandardsApiClient : ApiClient
    {
        public StandardsApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<IEnumerable<StandardOptions>> GetOptionsForAllStandards()
        {
            return await Get<IEnumerable<StandardOptions>>("api/v1/standards/options");
        }

        public async Task<StandardOptions> GetOptionsForStandard(string standard)
        {
            return await Get<StandardOptions>($"api/v1/standards/options/{standard}");
        }
    }
}
