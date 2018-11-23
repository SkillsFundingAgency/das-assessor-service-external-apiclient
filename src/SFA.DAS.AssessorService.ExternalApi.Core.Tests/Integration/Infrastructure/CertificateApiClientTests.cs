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
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var status = new Status { CurrentStatus = "Draft" };
            var created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Test" };

            var certificate = new Certificate { CertificateData = certificateData, Status = status, Created = created };
            var certificateRequest = new CreateCertificate
            {
                Learner = certificate.CertificateData.Learner,
                Standard = certificate.CertificateData.Standard,
                LearningDetails = certificate.CertificateData.LearningDetails,
                PostalContact = certificate.CertificateData.PostalContact
            };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = certificate }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CreateCertificate> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
        }

        [Test]
        public async Task CreateCertificate_CertificateExists()
        {
            // arrange 
            var certificateRequest = Builder<CreateCertificate>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate already exists: 123456789" };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CreateCertificate> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task UpdateCertificate()
        {
            // arrange 
            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = "DRAFT CERTIFICATE")
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var status = new Status { CurrentStatus = "Draft" };
            var created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = "Test" };

            var certificate = new Certificate { CertificateData = certificateData, Status = status, Created = created };
            var certificateRequest = new UpdateCertificate
            {
                CertificateReference = certificate.CertificateData.CertificateReference,
                Learner = certificate.CertificateData.Learner,
                Standard = certificate.CertificateData.Standard,
                LearningDetails = certificate.CertificateData.LearningDetails,
                PostalContact = certificate.CertificateData.PostalContact
            };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = certificate }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<UpdateCertificate> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
        }

        [Test]
        public async Task UpdateCertificate_CertificateNotFound()
        {
            // arrange 
            var certificateRequest = Builder<UpdateCertificate>.CreateNew().With(cd => cd.CertificateReference = "NOT FOUND")
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate not found" };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<UpdateCertificate> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task UpdateCertificate_CertificateStatusInvalid()
        {
            // arrange 
            var certificateRequest = Builder<UpdateCertificate>.CreateNew().With(cd => cd.CertificateReference = "SUBMITTED CERTIFICATE")
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate is not in 'Draft' status" };

            var expectedResponse = new List<BatchCertificateResponse>
            {
                new BatchCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<UpdateCertificate> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task SubmitCertificate()
        {
            // arrange 
            var submitCertificate = new SubmitCertificate { Uln = 9876543210, FamilyName = "Blogs", StandardCode = 1 };

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = "DRAFT CERTIFICATE")
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().With(s => s.StandardCode = 1).Build())
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var status = new Status { CurrentStatus = "Submitted" };
            var created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = "Test" };
            var submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Test" };

            var certificate = new Certificate { CertificateData = certificateData, Status = status, Created = created, Submitted = submitted };

            var expectedResponse = new List<SubmitBatchCertificateResponse>
            {
                new SubmitBatchCertificateResponse { Certificate = certificate }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificate> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
        }

        [Test]
        public async Task SubmitCertificate_CertificateNotFound()
        {
            // arrange 
            var submitCertificate = new SubmitCertificate { Uln = 9876543210, FamilyName = "Blogs", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Certificate not found" };

            var expectedResponse = new List<SubmitBatchCertificateResponse>
            {
                new SubmitBatchCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificate> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task SubmitCertificate_CertificateStatusInvalid()
        {
            // arrange 
            var submitCertificate = new SubmitCertificate { Uln = 1234567890, FamilyName = "Blogs", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Certificate is not in 'Ready' status" };

            var expectedResponse = new List<SubmitBatchCertificateResponse>
            {
                new SubmitBatchCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificate> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task SubmitCertificate_InvalidFamilyName()
        {
            // arrange 
            var submitCertificate = new SubmitCertificate { Uln = 1234567890, FamilyName = "INVALID", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Cannot find apprentice with the specified Uln, FamilyName & StandardCode" };

            var expectedResponse = new List<SubmitBatchCertificateResponse>
            {
                new SubmitBatchCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificate> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
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
                Message = "Cannot delete a submitted Certificate"
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
                Message = "Cannot find apprentice with the specified Uln, FamilyName & StandardCode"
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
        public async Task GetCertificate()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            int standardcode = 1;
            string certificateReference = "123456790";

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = certificateReference)
                                                            .With(cd => cd.Standard = Builder<Standard>.CreateNew().With(s => s.StandardCode = 1).Build())
                                                            .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                            .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                            .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                            .Build();

            var status = new Status { CurrentStatus = "Submitted" };
            var created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = "Test" };
            var submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Test" };

            var expectedResponse = new Certificate { CertificateData = certificateData, Status = status, Created = created, Submitted = submitted };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/certificate/{uln}/{lastname}/{standardcode}")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new GetCertificate { Uln = uln, FamilyName = lastname, StandardCode = standardcode };
            var actual = await _ApiClient.GetCertificate(request);

            // assert
            Assert.That(actual.Error, Is.Null);
            Assert.That(actual.Certificate, Is.Not.Null);
        }

        [Test]
        public async Task GetCertificate_CertificateNotYetCreated()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            int standardcode = 4321;

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/certificate/{uln}/{lastname}/{standardcode}")
                .Respond(HttpStatusCode.NoContent, "application/json", string.Empty);

            // act
            var request = new GetCertificate { Uln = uln, FamilyName = lastname, StandardCode = standardcode };
            var actual = await _ApiClient.GetCertificate(request);

            // assert
            Assert.That(actual.Error, Is.Null);
            Assert.That(actual.Certificate, Is.Null);
        }

        [Test]
        public async Task GetCertificate_InvalidRequest()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "INVALID";
            int standardcode = 4321;

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Cannot find apprentice with the specified Uln, FamilyName & StandardCode"
            };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/certificate/{uln}/{lastname}/{standardcode}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new GetCertificate { Uln = uln, FamilyName = lastname, StandardCode = standardcode };
            var actual = await _ApiClient.GetCertificate(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
            Assert.That(actual.Certificate, Is.Null);
        }

    }
}
