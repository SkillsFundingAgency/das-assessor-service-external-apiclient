namespace SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CertificateApiClient : ApiClient
    {
        public CertificateApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<GetBatchCertificateResponse> GetCertificate(GetCertificate request)
        {
            var response = new GetBatchCertificateResponse
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

        public async Task<IEnumerable<BatchCertificateResponse>> CreateCertificates(IEnumerable<CreateCertificate> request)
        {
            return await Post<IEnumerable<CreateCertificate>, IEnumerable<BatchCertificateResponse>>("api/v1/certificate", request);
        }

        public async Task<IEnumerable<BatchCertificateResponse>> UpdateCertificates(IEnumerable<UpdateCertificate> request)
        {
            return await Put<IEnumerable<UpdateCertificate>, IEnumerable<BatchCertificateResponse>>("api/v1/certificate", request);
        }

        public async Task<IEnumerable<SubmitBatchCertificateResponse>> SubmitCertificates(IEnumerable<SubmitCertificate> request)
        {
            return await Post<IEnumerable<SubmitCertificate>, IEnumerable<SubmitBatchCertificateResponse>>("api/v1/certificate/submit", request);
        }

        public async Task<DeleteBatchCertificateResponse> DeleteCertificate(DeleteCertificate request)
        {
            var error = await Delete<ApiResponse>($"api/v1/certificate/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.CertificateReference}");

            return new DeleteBatchCertificateResponse
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
