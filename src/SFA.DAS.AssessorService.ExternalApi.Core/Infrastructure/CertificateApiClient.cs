namespace SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Response;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Request;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CertificateApiClient : ApiClient
    {
        public CertificateApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<GetCertificateResponse> GetCertificate(GetCertificateRequest request)
        {
            var response = new GetCertificateResponse
            {
                Uln = request.Uln,
                FamilyName = request.FamilyName,
                Standard = request.Standard
            };

            using (var apiResponse = await _httpClient.GetAsync($"api/v1/certificate/{request.Uln}/{request.FamilyName}/{request.Standard}"))
            {
                if (apiResponse.IsSuccessStatusCode)
                {
                    response.Certificate = await apiResponse.Content.ReadAsAsync<Certificate>();
                }
                else
                {
                    response.Error = await apiResponse.Content.ReadAsAsync<ApiResponse>();
                }
            }

            return response;
        }

        public async Task<IEnumerable<CreateCertificateResponse>> CreateCertificates(IEnumerable<CreateCertificateRequest> request)
        {
            return await Post<IEnumerable<CreateCertificateRequest>, IEnumerable<CreateCertificateResponse>>("api/v1/certificate", request);
        }

        public async Task<IEnumerable<UpdateCertificateResponse>> UpdateCertificates(IEnumerable<UpdateCertificateRequest> request)
        {
            return await Put<IEnumerable<UpdateCertificateRequest>, IEnumerable<UpdateCertificateResponse>>("api/v1/certificate", request);
        }

        public async Task<IEnumerable<SubmitCertificateResponse>> SubmitCertificates(IEnumerable<SubmitCertificateRequest> request)
        {
            return await Post<IEnumerable<SubmitCertificateRequest>, IEnumerable<SubmitCertificateResponse>>("api/v1/certificate/submit", request);
        }

        public async Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest request)
        {
            var error = await Delete<ApiResponse>($"api/v1/certificate/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.CertificateReference}");

            return new DeleteCertificateResponse
            {
                Uln = request.Uln,
                FamilyName = request.FamilyName,
                Standard = request.Standard,
                CertificateReference = request.CertificateReference,
                Error = error
            };
        }

        public async Task<IEnumerable<string>> GetGrades()
        {
            return await Get<IEnumerable<string>>("api/v1/certificate/grades");
        }
    }
}
