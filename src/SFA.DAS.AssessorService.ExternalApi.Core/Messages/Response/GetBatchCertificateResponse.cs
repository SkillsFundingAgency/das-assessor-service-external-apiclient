namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;

    public class GetBatchCertificateResponse
    {
        public long Uln { get; set; }
        public string Standard { get; set; }
        public string FamilyName { get; set; }

        public Certificate Certificate { get; set; }

        public ApiResponse Error { get; set; }
    }
}
