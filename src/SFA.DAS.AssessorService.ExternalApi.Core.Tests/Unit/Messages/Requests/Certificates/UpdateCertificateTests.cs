namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Unit.Messages.Requests.Certificates
{
    using FizzWare.NBuilder;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System.Linq;

    [TestFixture(Category = "Requests")]
    public class UpdateCertificateTests
    {
        [Test]
        public void CertificateReferenceMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificate = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = null)
                                                                        .With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            Assert.That(validationResults.First().ErrorMessage, Is.EqualTo("Enter the certificate reference").IgnoreCase);
        }

        [Test]
        public void LearnerMissing()
        {
            // arrange
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificate = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "TEST")
                                                                        .With(cd => cd.Learner = null)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            Assert.That(validationResults.First().ErrorMessage, Is.EqualTo("Learner is required").IgnoreCase);
        }

        [Test]
        public void StandardMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificate = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "TEST")
                                                                        .With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = null)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            Assert.That(validationResults.First().ErrorMessage, Is.EqualTo("Standard is required").IgnoreCase);
        }

        [Test]
        public void LearningDetailsMissing()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificate = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "TEST")
                                                                        .With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = null)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            Assert.That(validationResults.First().ErrorMessage, Is.EqualTo("LearningDetails is required").IgnoreCase);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("NOT-A-POSTCODE")]
        public void WhenPostalContactIsMissingOrInvalid_ThenRequestIsValid(
    string postcode)
        {
            // Arrange
            var learner = Builder<Learner>.CreateNew()
                .With(x => x.Uln = 1243567890)
                .Build();

            var standard = Builder<Standard>.CreateNew().Build();

            var learningDetails = Builder<LearningDetails>.CreateNew()
                .With(x => x.OverallGrade = "Pass")
                .Build();

            var _sut = new UpdateCertificateRequest
            {
                CertificateReference = "TEST",
                Learner = learner,
                Standard = standard,
                LearningDetails = learningDetails,
                PostalContact = postcode is null
                ? null
                : new PostalContact { PostCode = postcode }
            };

            // Act
            var isValid = _sut.IsValid(out var validationResults);

            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(validationResults, Is.Empty);
        }

        [Test]
        public void WhenValid()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();

            var certificate = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "TEST")
                                                                        .With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.That(isValid, Is.True);
            Assert.That(validationResults, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("IEquatable")]
        public void WhenEqual()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();

            var certificate1 = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "TEST")
                                                                        .With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate2 = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "TEST")
                                                                        .With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool areEqual = certificate1 == certificate2;

            // assert
             Assert.That(areEqual, Is.True);
        }

        [Test]
        [Category("IEquatable")]
        public void WhenNotEqual()
        {
            // arrange
            var learner = Builder<Learner>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();

            var certificate1 = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "TEST 123")
                                                                        .With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate2 = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "TEST 321")
                                                                        .With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            // act
            bool areNotEqual = certificate1 != certificate2;

            // assert
            Assert.That(areNotEqual, Is.True);
        }
    }
}
