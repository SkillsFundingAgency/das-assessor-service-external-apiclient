namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models
{

    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Linq;

    [TestFixture(Category = "Model")]
    public class CertificateStatusTests
    {
        [Test]
        public void CurrentStatusMissing()
        {
            // arrange
            var status = Builder<CertificateStatus>.CreateNew().With(c => c.CurrentStatus = null).Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("CurrentStatus is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void CreatedByMissing()
        {
            // arrange
            var status = Builder<CertificateStatus>.CreateNew().With(c => c.CreatedBy = null).Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("CreatedBy is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var status = Builder<CertificateStatus>.CreateNew().With(s => s.CurrentStatus = "Draft").With(s => s.CreatedBy = "Test").Build();

            // act
            bool isValid = status.IsValid(out var validationResults);

            // assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var status1 = Builder<CertificateStatus>.CreateNew().With(s => s.CurrentStatus = "Draft").With(s => s.CreatedBy = "Test").Build();
            var status2 = Builder<CertificateStatus>.CreateNew().With(s => s.CurrentStatus = "Draft").With(s => s.CreatedBy = "Test").Build();

            // act
            bool areEqual = status1 == status2;

            // assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var status1 = Builder<CertificateStatus>.CreateNew().With(s => s.CurrentStatus = "Draft").With(s => s.CreatedBy = "Test").Build();
            var status2 = Builder<CertificateStatus>.CreateNew().With(s => s.CurrentStatus = "Submitted").With(s => s.CreatedBy = "Test").Build();

            // act
            bool areNotEqual = status1 != status2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
