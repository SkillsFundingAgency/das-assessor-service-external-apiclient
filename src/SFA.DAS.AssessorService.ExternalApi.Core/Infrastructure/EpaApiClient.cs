namespace SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Epa;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class EpaApiClient : ApiClient
    {
        public EpaApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<IEnumerable<CreateEpaResponse>> CreateEpaRecords(IEnumerable<CreateEpaRequest> request)
        {
            return await Post<IEnumerable<CreateEpaRequest>, IEnumerable<CreateEpaResponse>>("api/v1/epa", request);
        }

        public async Task<IEnumerable<UpdateEpaResponse>> UpdateEpaRecordss(IEnumerable<UpdateEpaRequest> request)
        {
            return await Put<IEnumerable<UpdateEpaRequest>, IEnumerable<UpdateEpaResponse>>("api/v1/epa", request);
        }

        public async Task<DeleteEpaResponse> DeleteEpaRecord(DeleteEpaRequest request)
        {
            var error = await Delete<ApiResponse>($"api/v1/epa/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.EpaReference}");

            return new DeleteEpaResponse
            {
                Uln = request.Uln,
                FamilyName = request.FamilyName,
                Standard = request.Standard,
                EpaReference = request.EpaReference,
                Error = error
            };
        }
    }
}
