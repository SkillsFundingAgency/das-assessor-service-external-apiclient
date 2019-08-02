namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models.Learners
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners;

    [TestFixture(Category = "Models")]
    public class StatusTests
    {
        [Test]
        public void WhenStatusValid()
        {
            // arrange
            var status = Builder<Status>.CreateNew().With(s => s.CompletionStatus = 1).Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenStatusEqual()
        {
            // arrange
            var status1 = Builder<Status>.CreateNew().Build();
            var status2 = Builder<Status>.CreateNew().Build();

            // act
            bool areEqual = status1 == status2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenStatusNotEqual()
        {
            // arrange
            var status1 = Builder<Status>.CreateNew().With(s => s.CompletionStatus = 1).Build();
            var status2 = Builder<Status>.CreateNew().With(s => s.CompletionStatus = 2).Build();

            // act
            bool areNotEqual = status1 != status2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
