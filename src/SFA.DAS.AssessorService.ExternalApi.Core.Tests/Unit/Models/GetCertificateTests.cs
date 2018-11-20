namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Linq;

    [TestFixture(Category = "Model")]
    public class GetCertificateTests
    {
        [Test]
        public void UlnInvalid()
        {
            // arrange
            var certificate = Builder<GetCertificate>.CreateNew().With(sc => sc.Uln = 12435).With(sc => sc.StandardCode = 1).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("The apprentice's ULN should contain exactly 10 numbers", validationResults.First().ErrorMessage);
        }

        [Test]
        public void InvalidStandardCode()
        {
            // arrange
            var certificate = Builder<GetCertificate>.CreateNew().With(sc => sc.Uln = 1243567890).With(sc => sc.StandardCode = -1).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("A standard should be selected", validationResults.First().ErrorMessage);
        }

        [Test]
        public void FamilyNameMissing()
        {
            // arrange
            var certificate = Builder<GetCertificate>.CreateNew().With(sc => sc.Uln = 1243567890).With(sc => sc.StandardCode = 1).With(l => l.FamilyName = null).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Enter the apprentice's last name", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var certificate = Builder<GetCertificate>.CreateNew().With(sc => sc.Uln = 1243567890).With(sc => sc.StandardCode = 1).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var certificate1 = Builder<GetCertificate>.CreateNew().With(sc => sc.StandardCode = 1).Build();
            var certificate2 = Builder<GetCertificate>.CreateNew().With(sc => sc.StandardCode = 1).Build();

            // act
            bool areEqual = certificate1 == certificate2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var certificate1 = Builder<GetCertificate>.CreateNew().With(sc => sc.StandardCode = 1).Build();
            var certificate2 = Builder<GetCertificate>.CreateNew().With(sc => sc.StandardCode = 9).Build();

            // act
            bool areNotEqual = certificate1 != certificate2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
