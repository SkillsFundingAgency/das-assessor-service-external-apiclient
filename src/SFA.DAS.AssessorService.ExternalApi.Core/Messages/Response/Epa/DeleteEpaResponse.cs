namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Epa
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;

    public class DeleteEpaResponse
    {
        public long Uln { get; set; }
        public string Standard { get; set; }
        public string FamilyName { get; set; }
        public string EpaReference { get; set; }

        public ApiResponse Error { get; set; }
    }
}
