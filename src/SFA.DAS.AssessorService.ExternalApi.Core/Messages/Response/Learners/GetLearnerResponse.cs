namespace SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Learners
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners;

    public class GetLearnerResponse
    {
        public long Uln { get; set; }
        public string Standard { get; set; }
        public string FamilyName { get; set; }

        public Learner Learner { get; set; }

        public ApiResponse Error { get; set; }
    }
}
