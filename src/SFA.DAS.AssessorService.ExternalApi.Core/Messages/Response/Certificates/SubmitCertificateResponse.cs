namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Certificates
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Collections.Generic;

    public class SubmitCertificateResponse
    {
        public string RequestId { get; set; }

        public Certificate Certificate { get; set; }

        public IEnumerable<string> ValidationErrors { get; set; } = new List<string>();
    }
}
