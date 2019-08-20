namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models.Learners
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners;

    [TestFixture(Category = "Models")]
    public class LearnerTests
    {
        [Test]
        public void WhenLearnerValid()
        {
            // arrange
            var status = Builder<Learner>.CreateNew().Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenLearnerEqual()
        {
            // arrange
            var status1 = Builder<Learner>.CreateNew().Build();
            var status2 = Builder<Learner>.CreateNew().Build();

            // act
            bool areEqual = status1 == status2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenLearnerNotEqual()
        {
            // arrange
            var status1 = Builder<Learner>.CreateNew().With(s => s.Status = null).Build();
            var status2 = Builder<Learner>.CreateNew().With(s => s.Status = Builder<Status>.CreateNew().Build()).Build();

            // act
            bool areNotEqual = status1 != status2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
