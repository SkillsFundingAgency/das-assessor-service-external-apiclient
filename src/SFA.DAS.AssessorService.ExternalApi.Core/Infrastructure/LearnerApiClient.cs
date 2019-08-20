namespace SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Learners;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Learners;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class LearnerApiClient : ApiClient
    {
        public LearnerApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<GetLearnerResponse> GetLearner(GetLearnerRequest request)
        {
            var response = new GetLearnerResponse
            {
                Uln = request.Uln,
                FamilyName = request.FamilyName,
                Standard = request.Standard
            };

            using (var apiResponse = await _httpClient.GetAsync($"api/v1/learner/{request.Uln}/{request.FamilyName}/{request.Standard}"))
            {
                if (apiResponse.IsSuccessStatusCode)
                {
                    response.Learner = await apiResponse.Content.ReadAsAsync<Learner>();
                }
                else
                {
                    response.Error = await apiResponse.Content.ReadAsAsync<ApiResponse>();
                }
            }

            return response;
        }
    }
}
