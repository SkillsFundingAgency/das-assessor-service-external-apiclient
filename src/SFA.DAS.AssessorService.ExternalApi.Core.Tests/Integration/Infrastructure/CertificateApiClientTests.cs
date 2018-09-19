namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Integration.Infrastructure
{
    using FizzWare.NBuilder;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    [TestFixture(Category = "Infrastructure")]
    public sealed class CertificateApiClientTests : IDisposable
    {
        private const string apiBaseAddress = "http://localhost";
        private const string subscriptionKey = "test";

        private CertificateApiClient _ApiClient;
        private MockHttpMessageHandler _MockHttp;

        [SetUp]
        public void Setup()
        {
            _MockHttp = new MockHttpMessageHandler();

            HttpClient httpClient = _MockHttp.ToHttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            httpClient.BaseAddress = new Uri(apiBaseAddress);

            _ApiClient = new CertificateApiClient(httpClient);
        }

        [TearDown]
        public void Dispose()
        {
            _MockHttp?.Dispose();
        }

        [Test]
        public async Task CreateCertificate()
        {
            // arrange 
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var certificate = new Certificate { CertificateData = certificateData, Status = "Draft", CreatedAt = DateTime.UtcNow, CreatedBy = "Test" };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = certificate, ProvidedCertificateData = certificateData}
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CertificateData> { certificateData });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
            Assert.That(actual.First().ProvidedCertificateData, Is.EqualTo(expectedResponse.First().ProvidedCertificateData));
        }

        [Test]
        public async Task CreateCertificate_CertificateExists()
        {
            // arrange 
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate already exists: 123456789" };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = null, ProvidedCertificateData = certificateData, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CertificateData> { certificateData });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
            Assert.That(actual.First().ProvidedCertificateData, Is.EqualTo(expectedResponse.First().ProvidedCertificateData));
        }

        [Test]
        public async Task UpdateCertificate()
        {
            // arrange 
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = "DRAFT CERTIFICATE")
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var certificate = new Certificate { CertificateData = certificateData, Status = "Draft", CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = "Test", UpdatedAt = DateTime.UtcNow, UpdatedBy = "Test" };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = certificate, ProvidedCertificateData = certificateData}
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<CertificateData> { certificateData });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
            Assert.That(actual.First().ProvidedCertificateData, Is.EqualTo(expectedResponse.First().ProvidedCertificateData));
        }

        [Test]
        public async Task UpdateCertificate_CertificateNotFound()
        {
            // arrange 
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = "NOT FOUND")
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate not found" };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = null, ProvidedCertificateData = certificateData, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<CertificateData> { certificateData });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
            Assert.That(actual.First().ProvidedCertificateData, Is.EqualTo(expectedResponse.First().ProvidedCertificateData));
        }

        [Test]
        public async Task UpdateCertificate_CertificateStatusInvalid()
        {
            // arrange 
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = "SUBMITTED CERTIFICATE")
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate is not in 'Draft' status" };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = null, ProvidedCertificateData = certificateData, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<CertificateData> { certificateData });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
            Assert.That(actual.First().ProvidedCertificateData, Is.EqualTo(expectedResponse.First().ProvidedCertificateData));
        }

        [Test]
        public async Task SubmitCertificate()
        {
            // arrange 
            var submitCertificate = new SubmitCertificate { Uln = 9876543210, FamilyName = "Blogs", StandardCode = 1 };

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = "DRAFT CERTIFICATE")
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var certificate = new Certificate { CertificateData = certificateData, Status = "Submitted", CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = "Test", UpdatedAt = DateTime.UtcNow, UpdatedBy = "Test" };

            var expectedResponse = new List<SubmitBatchCertificateResponse>
            {
                new SubmitBatchCertificateResponse { Certificate = certificate, Uln = 9876543210, FamilyName = "Blogs", StandardCode = 1}
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificate> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
            Assert.That(actual.First().Uln, Is.EqualTo(expectedResponse.First().Uln));
            Assert.That(actual.First().FamilyName, Is.EqualTo(expectedResponse.First().FamilyName));
            Assert.That(actual.First().StandardCode, Is.EqualTo(expectedResponse.First().StandardCode));
        }

        [Test]
        public async Task SubmitCertificate_CertificateNotFound()
        {
            // arrange 
            var submitCertificate = new SubmitCertificate { Uln = 9876543210, FamilyName = "Blogs", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Certificate not found" };

            var expectedResponse = new List<SubmitBatchCertificateResponse>
            {
                new SubmitBatchCertificateResponse { Certificate = null, Uln = 9876543210, FamilyName = "Blogs", StandardCode = 1, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificate> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
            Assert.That(actual.First().Uln, Is.EqualTo(expectedResponse.First().Uln));
            Assert.That(actual.First().FamilyName, Is.EqualTo(expectedResponse.First().FamilyName));
            Assert.That(actual.First().StandardCode, Is.EqualTo(expectedResponse.First().StandardCode));
        }

        [Test]
        public async Task SubmitCertificate_CertificateStatusInvalid()
        {
            // arrange 
            var submitCertificate = new SubmitCertificate { Uln = 1234567890, FamilyName = "Blogs", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Certificate is not in 'Draft' or 'Ready' status" };

            var expectedResponse = new List<SubmitBatchCertificateResponse>
            {
                new SubmitBatchCertificateResponse { Certificate = null, Uln = 1234567890, FamilyName = "Blogs", StandardCode = 1, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificate> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
            Assert.That(actual.First().Uln, Is.EqualTo(expectedResponse.First().Uln));
            Assert.That(actual.First().FamilyName, Is.EqualTo(expectedResponse.First().FamilyName));
            Assert.That(actual.First().StandardCode, Is.EqualTo(expectedResponse.First().StandardCode));
        }

        [Test]
        public async Task SubmitCertificate_InvalidFamilyName()
        {
            // arrange 
            var submitCertificate = new SubmitCertificate { Uln = 1234567890, FamilyName = "INVALID", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Cannot find entry for specified Uln, FamilyName & StandardCode" };

            var expectedResponse = new List<SubmitBatchCertificateResponse>
            {
                new SubmitBatchCertificateResponse { Certificate = null, Uln = 1234567890, FamilyName = "INVALID", StandardCode = 1, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificate> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
            Assert.That(actual.First().Uln, Is.EqualTo(expectedResponse.First().Uln));
            Assert.That(actual.First().FamilyName, Is.EqualTo(expectedResponse.First().FamilyName));
            Assert.That(actual.First().StandardCode, Is.EqualTo(expectedResponse.First().StandardCode));
        }

        [Test]
        public async Task DeleteCertificate()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            int standardcode = 1;
            string certificateReference = "123456790";

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/certificate/{uln}/{lastname}/{standardcode}/{certificateReference}")
                .Respond(HttpStatusCode.OK, "application/json", string.Empty);

            // act
            var request = new DeleteCertificate { Uln = uln, FamilyName = lastname, StandardCode = standardcode, CertificateReference = certificateReference };
            var actual = await _ApiClient.DeleteCertificate(request);

            // assert
            Assert.That(actual.Error, Is.Null);
        }

        [Test]
        public async Task DeleteCertificate_CertificateNotFound()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            int standardcode = 4321;
            string certificateReference = "1234567890";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Certificate not found"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/certificate/{uln}/{lastname}/{standardcode}/{certificateReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteCertificate { Uln = uln, FamilyName = lastname, StandardCode = standardcode, CertificateReference = certificateReference };
            var actual = await _ApiClient.DeleteCertificate(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
        }

        [Test]
        public async Task DeleteCertificate_CertificateStatusInvalid()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            int standardcode = 1;
            string certificateReference = "1234567890";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Certificate cannot be Deleted when in 'Submitted' status"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/certificate/{uln}/{lastname}/{standardcode}/{certificateReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteCertificate { Uln = uln, FamilyName = lastname, StandardCode = standardcode, CertificateReference = certificateReference };
            var actual = await _ApiClient.DeleteCertificate(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
        }

        [Test]
        public async Task DeleteCertificate_InvalidFamilyName()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "INVALID";
            int standardcode = 1;
            string certificateReference = "1234567890";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Cannot find entry for specified Uln, FamilyName & StandardCode"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/certificate/{uln}/{lastname}/{standardcode}/{certificateReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteCertificate { Uln = uln, FamilyName = lastname, StandardCode = standardcode, CertificateReference = certificateReference };
            var actual = await _ApiClient.DeleteCertificate(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
        }
    }
}
