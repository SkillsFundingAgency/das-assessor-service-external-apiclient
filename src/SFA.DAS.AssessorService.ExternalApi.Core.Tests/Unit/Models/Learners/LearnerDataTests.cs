namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models.Learners
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners;

    [TestFixture(Category = "Models")]
    public class LearnerDataTests
    {
        [Test]
        public void WhenLearnerDataValid()
        {
            // arrange
            var status = Builder<LearnerData>.CreateNew().Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenLearnerDataEqual()
        {
            // arrange
            var status1 = Builder<LearnerData>.CreateNew().Build();
            var status2 = Builder<LearnerData>.CreateNew().Build();

            // act
            bool areEqual = status1 == status2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenLearnerDataNotEqual()
        {
            // arrange
            var status1 = Builder<LearnerData>.CreateNew().With(s => s.Standard = null).Build();
            var status2 = Builder<LearnerData>.CreateNew().With(s => s.Standard = Builder<Standard>.CreateNew().Build()).Build();

            // act
            bool areNotEqual = status1 != status2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
