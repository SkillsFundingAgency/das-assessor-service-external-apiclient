namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models.Learners
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Learners;
    using System;

    [TestFixture(Category = "Models")]
    public class LearningDetailsTests
    {
        [Test]
        public void WhenLearningDetailsValid()
        {
            // arrange
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();

            // act
            bool isValid = learningDetails.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenLearningDetailsEqual()
        {
            // arrange
            var learningDetails1 = Builder<LearningDetails>.CreateNew().Build();
            var learningDetails2 = Builder<LearningDetails>.CreateNew().Build();

            // act
            bool areEqual = learningDetails1 == learningDetails2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenLearningDetailsNotEqual()
        {
            // arrange
            var learningDetails1 = Builder<LearningDetails>.CreateNew().With(ld => ld.LearningStartDate = DateTime.MinValue).Build();
            var learningDetails2 = Builder<LearningDetails>.CreateNew().With(ld => ld.LearningStartDate = DateTime.MaxValue).Build();

            // act
            bool areNotEqual = learningDetails1 != learningDetails2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
