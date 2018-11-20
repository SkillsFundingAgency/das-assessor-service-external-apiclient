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
            var status = Builder<Status>.CreateNew().Build();
            var certificate = Builder<Certificate>.CreateNew().With(c => c.Status = status).With(c => c.CertificateData = null).Build();

            // act
            bool isValid = certificate.IsValid(out var validationResults);

            // assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.Count.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("CertificateData is required", validationResults.First().ErrorMessage);
        }


        [Test]
        public void WhenValid()
        {
            // arrange
            var status = Builder<Status>.CreateNew().Build();
            var learner = Builder<Learner>.CreateNew().With(l => l.Uln = 1243567890).Build();
            var standard = Builder<Standard>.CreateNew().With(l => l.StandardCode = 1).Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().With(l => l.OverallGrade = "Pass").Build();
            var postalContact = Builder<PostalContact>.CreateNew().With(l => l.PostCode = "ZY9 9ZY").Build();
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate = Builder<Certificate>.CreateNew().With(c => c.Status = status).With(c => c.CertificateData = certificateData).Build();

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
            var status = Builder<Status>.CreateNew().Build();
            var learner = Builder<Learner>.CreateNew().Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate1 = Builder<Certificate>.CreateNew().With(c => c.Status = status).With(c => c.CertificateData = certificateData).Build();
            var certificate2 = Builder<Certificate>.CreateNew().With(c => c.Status = status).With(c => c.CertificateData = certificateData).Build();

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
            var status1 = Builder<Status>.CreateNew().With(c => c.CurrentStatus = "Draft").Build();
            var status2 = Builder<Status>.CreateNew().With(c => c.CurrentStatus = "Deleted").Build();

            var learner = Builder<Learner>.CreateNew().Build();
            var standard = Builder<Standard>.CreateNew().Build();
            var learningDetails = Builder<LearningDetails>.CreateNew().Build();
            var postalContact = Builder<PostalContact>.CreateNew().Build();
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = learner)
                                                                        .With(cd => cd.Standard = standard)
                                                                        .With(cd => cd.LearningDetails = learningDetails)
                                                                        .With(cd => cd.PostalContact = postalContact).Build();

            var certificate1 = Builder<Certificate>.CreateNew().With(c => c.Status = status1).With(c => c.CertificateData = certificateData).Build();
            var certificate2 = Builder<Certificate>.CreateNew().With(c => c.Status = status2).With(c => c.CertificateData = certificateData).Build();

            // act
            bool areNotEqual = certificate1 != certificate2;

            // assert
            Assert.IsTrue(areNotEqual);
        }
    }
}
