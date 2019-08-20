namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Epa
{
    using System.Collections.Generic;

    public class UpdateEpaResponse
    {
        public string RequestId { get; set; }

        public string EpaReference { get; set; }

        public IEnumerable<string> ValidationErrors { get; set; } = new List<string>();
    }
}
