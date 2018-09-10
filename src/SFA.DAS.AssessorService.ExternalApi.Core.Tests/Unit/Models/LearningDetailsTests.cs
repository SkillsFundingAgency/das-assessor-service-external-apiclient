namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Linq;

    [TestFixture(Category = "Model")]
    public class LearningDetailsTests
    {
        [Test]
        public void AchievementDateBeforeDigitalCertificates()
        {
            // arrange
            DateTime firstDigitalCertificate = new DateTime(2017, 1, 1);
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.AchievementDate = firstDigitalCertificate.AddDays(-1)).Build();

            // act
            bool isValid = learningDetails.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("An achievement date cannot be before 01 01 2017", validationResults.First().ErrorMessage);
        }

        [Test]
        public void AchievementDateInFuture()
        {
            // arrange
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.AchievementDate = DateTime.UtcNow.AddDays(1)).Build();

            // act
            bool isValid = learningDetails.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("An achievement date cannot be in the future", validationResults.First().ErrorMessage);
        }

        [Test]
        public void AchievementDateMissing()
        {
            // arrange
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.AchievementDate = null).Build();

            // act
            bool isValid = learningDetails.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter the achievement date", validationResults.First().ErrorMessage);
        }

        [Test]
        public void OverallGradeMissing()
        {
            // arrange
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.AchievementDate = DateTime.UtcNow).With(l => l.OverallGrade = null).Build();

            // act
            bool isValid = learningDetails.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter an overall grade", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.AchievementDate = DateTime.UtcNow).Build();

            // act
            bool isValid = learningDetails.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
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
        public void WhenNotEqual()
        {
            // arrange
            var learningDetails1 = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "PASS").Build();
            var learningDetails2 = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "FAIL").Build();

            // act
            bool areNotEqual = learningDetails1 != learningDetails2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
