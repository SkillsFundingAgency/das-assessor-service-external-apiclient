namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;

    public class DeleteBatchCertificateResponse
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string FamilyName { get; set; }

        public ApiResponse Error { get; set; }
    }
}