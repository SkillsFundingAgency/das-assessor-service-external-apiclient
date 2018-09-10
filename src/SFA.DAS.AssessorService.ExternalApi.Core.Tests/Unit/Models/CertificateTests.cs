namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Models
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Linq;

    [TestFixture(Category = "Model")]
    public class CertificateTests
    {
        [Test]
        public void CertificateDataMissing()
        {
            // arrange
            var certificate = Builder<Certificate>.CreateNew().With(c => c.CertificateData = null).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("CertificateData is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void StatusMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate = Builder<Certificate>.CreateNew().With(c => c.Status = null).With(c => c.CertificateData = certificateData).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Status is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void CreatedByMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate = Builder<Certificate>.CreateNew().With(c => c.CreatedBy = null).With(c => c.CertificateData = certificateData).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("CreatedBy is required", validationResults.First().ErrorMessage);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate = Builder<Certificate>.CreateNew().With(c => c.CertificateData = certificateData).Build();

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
            var learner = Builder<Learner>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate1 = Builder<Certificate>.CreateNew().With(c => c.CertificateData = certificateData).Build();
            var certificate2 = Builder<Certificate>.CreateNew().With(c => c.CertificateData = certificateData).Build();

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
            var learner = Builder<Learner>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate1 = Builder<Certificate>.CreateNew().With(c=> c.BatchNumber = 1).With(c => c.CertificateData = certificateData).Build();
            var certificate2 = Builder<Certificate>.CreateNew().With(c => c.BatchNumber = 2).With(c => c.CertificateData = certificateData).Build();

            // act
            bool areNotEqual = certificate1 != certificate2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
