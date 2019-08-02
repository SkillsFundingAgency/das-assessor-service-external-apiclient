namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Messages.Requests.Learners
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Learners;
    using System.Linq;

    [TestFixture(Category = "Requests")]
    public class GetLearnerTests
    {
        [Test]
        public void UlnInvalid()
        {
            // arrange
            var request = Builder<GetLearnerRequest>.CreateNew().With(r => r.Uln = 12435).Build();

            // act
            bool isValid = request.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("The apprentice's ULN should contain exactly 10 numbers", validationResults.First().ErrorMessage);
        }

        [Test]
        public void NoStandardSpecified()
        {
            // arrange
            var request = Builder<GetLearnerRequest>.CreateNew().With(r => r.Uln = 1243567890).With(r => r.Standard = null).Build();

            // act
            bool isValid = request.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("A standard should be selected", validationResults.First().ErrorMessage);
        }

        [Test]
        public void FamilyNameMissing()
        {
            // arrange
            var request = Builder<GetLearnerRequest>.CreateNew().With(r => r.Uln = 1243567890).With(r => r.FamilyName = null).Build();

            // act
            bool isValid = request.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter the apprentice's last name", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var request = Builder<GetLearnerRequest>.CreateNew().With(r => r.Uln = 1243567890).Build();

            // act
            bool isValid = request.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var request1 = Builder<GetLearnerRequest>.CreateNew().With(r => r.Standard = "1").Build();
            var request2 = Builder<GetLearnerRequest>.CreateNew().With(r => r.Standard = "1").Build();

            // act
            bool areEqual = request1 == request2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var request1 = Builder<GetLearnerRequest>.CreateNew().With(r => r.Standard = "1").Build();
            var request2 = Builder<GetLearnerRequest>.CreateNew().With(r => r.Standard = "9").Build();

            // act
            bool areNotEqual = request1 != request2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
