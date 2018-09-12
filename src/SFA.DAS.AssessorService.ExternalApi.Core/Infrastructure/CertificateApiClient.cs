namespace SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CertificateApiClient : ApiClient
    {
        public CertificateApiClient(HttpClient httpClient) : base(httpClient) { }

        public async Task<IEnumerable<BatchCertificateResponse>> CreateCertificates(IEnumerable<CertificateData> request)
        {
            return await Put<IEnumerable<CertificateData>, IEnumerable<BatchCertificateResponse>>("certificate", request);
        }

        public async Task<IEnumerable<BatchCertificateResponse>> UpdateCertificates(IEnumerable<CertificateData> request)
        {
            return await Post<IEnumerable<CertificateData>, IEnumerable<BatchCertificateResponse>>("certificate", request);
        }

        public async Task<IEnumerable<SubmitBatchCertificateResponse>> SubmitCertificates(IEnumerable<SubmitCertificate> request)
        {
            return await Post<IEnumerable<SubmitCertificate>, IEnumerable<SubmitBatchCertificateResponse>>("certificate/submit", request);
        }

        public async Task<object> DeleteCertificate(long uln, string lastname, int standardCode)
        {
            return await Delete<object>($"certificate/{uln}/{lastname}/{standardCode}");
        }
    }
}
