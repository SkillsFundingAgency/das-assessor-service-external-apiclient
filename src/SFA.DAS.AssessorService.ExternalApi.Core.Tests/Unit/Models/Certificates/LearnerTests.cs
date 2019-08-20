namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models.Certificates
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Linq;

    [TestFixture(Category = "Models")]
    public class LearnerTests
    {
        [Test]
        public void UlnInvalid()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 12435).Build();

            // act
            bool isValid = learner.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("The apprentice's ULN should contain exactly 10 numbers", validationResults.First().ErrorMessage);
        }

        [Test]
        public void GivenNamesMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).With(l => l.GivenNames = null).Build();

            // act
            bool isValid = learner.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter the apprentice's first name", validationResults.First().ErrorMessage);
        }

        [Test]
        public void FamilyNameMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).With(l => l.FamilyName = null).Build();

            // act
            bool isValid = learner.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter the apprentice's last name", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();

            // act
            bool isValid = learner.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var learner1 = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var learner2 = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();

            // act
            bool areEqual = learner1 == learner2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var learner1 = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var learner2 = Builder<Learner>.CreateNew().With(l => l.Uln = 9876543210).Build();

            // act
            bool areNotEqual = learner1 != learner2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
