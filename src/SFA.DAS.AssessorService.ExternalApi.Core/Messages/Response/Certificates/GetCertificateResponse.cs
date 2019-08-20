namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Certificates
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;

    public class GetCertificateResponse
    {
        public long Uln { get; set; }
        public string Standard { get; set; }
        public string FamilyName { get; set; }

        public Certificate Certificate { get; set; }

        public ApiResponse Error { get; set; }
    }
}
