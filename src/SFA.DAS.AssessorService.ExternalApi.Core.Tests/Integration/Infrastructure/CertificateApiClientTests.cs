namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Integration.Infrastructure
{
    using FizzWare.NBuilder;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Certificates;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Certificates;
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
            var certificateRequest = new CreateCertificateRequest
            {
                Learner = certificate.CertificateData.Learner,
                Standard = certificate.CertificateData.Standard,
                LearningDetails = certificate.CertificateData.LearningDetails,
                PostalContact = certificate.CertificateData.PostalContact
            };

            var expectedResponse = new List<CreateCertificateResponse>
            {
                new CreateCertificateResponse { Certificate = certificate }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CreateCertificateRequest> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
        }

        [Test]
        public async Task CreateCertificate_CertificateExists()
        {
            // arrange 
            var certificateRequest = Builder<CreateCertificateRequest>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate already exists: 123456789" };

            var expectedResponse = new List<CreateCertificateResponse>
            {
                new CreateCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CreateCertificateRequest> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task CreateCertificate_InvalidCourseOption()
        {
            // arrange 
            var certificateRequest = Builder<CreateCertificateRequest>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().With(s => s.StandardCode = 1).Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().With(ld => ld.CourseOption = "INVALID").Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Invalid course option for this Standard. Must be one of the following: English, French, German" };

            var expectedResponse = new List<CreateCertificateResponse>
            {
                new CreateCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CreateCertificateRequest> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task CreateCertificate_InvalidGrade()
        {
            // arrange 
            var certificateRequest = Builder<CreateCertificateRequest>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().With(ld => ld.OverallGrade = "INVALID").Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Invalid grade. Must be one of the following: Pass, Credit, Merit, Distinction, Pass with excellence, No grade awarded" };

            var expectedResponse = new List<CreateCertificateResponse>
            {
                new CreateCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CreateCertificateRequest> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task CreateCertificate_Using_StandardCode_NotFound()
        {
            // arrange 
            var certificateRequest = Builder<CreateCertificateRequest>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().With(s => s.StandardCode = 5555).Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Unable to find specified Standard" };

            var expectedResponse = new List<CreateCertificateResponse>
            {
                new CreateCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CreateCertificateRequest> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task CreateCertificate_Using_StandardReference_NotFound()
        {
            // arrange 
            var certificateRequest = Builder<CreateCertificateRequest>.CreateNew().With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().With(s => s.StandardReference = "INVALID").Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Unable to find specified Standard" };

            var expectedResponse = new List<CreateCertificateResponse>
            {
                new CreateCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateCertificates(new List<CreateCertificateRequest> { certificateRequest });

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
            var certificateRequest = new UpdateCertificateRequest
            {
                CertificateReference = certificate.CertificateData.CertificateReference,
                Learner = certificate.CertificateData.Learner,
                Standard = certificate.CertificateData.Standard,
                LearningDetails = certificate.CertificateData.LearningDetails,
                PostalContact = certificate.CertificateData.PostalContact
            };

            var expectedResponse = new List<UpdateCertificateResponse>
            {
                new UpdateCertificateResponse { Certificate = certificate }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<UpdateCertificateRequest> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
        }

        [Test]
        public async Task UpdateCertificate_CertificateNotFound()
        {
            // arrange 
            var certificateRequest = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "NOT FOUND")
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate not found" };

            var expectedResponse = new List<UpdateCertificateResponse>
            {
                new UpdateCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<UpdateCertificateRequest> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task UpdateCertificate_CertificateStatusInvalid()
        {
            // arrange 
            var certificateRequest = Builder<UpdateCertificateRequest>.CreateNew().With(cd => cd.CertificateReference = "SUBMITTED CERTIFICATE")
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().Build())
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var expectedValidationErrors = new List<string> { "Certificate is not in 'Draft' status" };

            var expectedResponse = new List<UpdateCertificateResponse>
            {
                new UpdateCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/api/v1/certificate")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateCertificates(new List<UpdateCertificateRequest> { certificateRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task SubmitCertificate_Using_StandardCode()
        {
            // arrange 
            var submitCertificate = new SubmitCertificateRequest { Uln = 9876543210, FamilyName = "Blogs", StandardCode = 1 };

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = "DRAFT CERTIFICATE")
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().With(s => s.StandardCode = 1).With(s => s.StandardReference = null).Build())
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var status = new Status { CurrentStatus = "Submitted" };
            var created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = "Test" };
            var submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Test" };

            var certificate = new Certificate { CertificateData = certificateData, Status = status, Created = created, Submitted = submitted };

            var expectedResponse = new List<SubmitCertificateResponse>
            {
                new SubmitCertificateResponse { Certificate = certificate }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificateRequest> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
        }

        [Test]
        public async Task SubmitCertificate_Using_StandardReference()
        {
            // arrange 
            var submitCertificate = new SubmitCertificateRequest { Uln = 9876543210, FamilyName = "Blogs", StandardReference = "1" };

            var certificateData = Builder<CertificateData>.CreateNew().With(cd => cd.CertificateReference = "DRAFT CERTIFICATE")
                                                                        .With(cd => cd.Standard = Builder<Standard>.CreateNew().With(s => s.StandardCode = null).With(s => s.StandardReference = "1").Build())
                                                                        .With(cd => cd.Learner = Builder<Learner>.CreateNew().Build())
                                                                        .With(cd => cd.LearningDetails = Builder<LearningDetails>.CreateNew().Build())
                                                                        .With(cd => cd.PostalContact = Builder<PostalContact>.CreateNew().Build())
                                                                        .Build();

            var status = new Status { CurrentStatus = "Submitted" };
            var created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = "Test" };
            var submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Test" };

            var certificate = new Certificate { CertificateData = certificateData, Status = status, Created = created, Submitted = submitted };

            var expectedResponse = new List<SubmitCertificateResponse>
            {
                new SubmitCertificateResponse { Certificate = certificate }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificateRequest> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().Certificate, Is.EqualTo(expectedResponse.First().Certificate));
        }

        [Test]
        public async Task SubmitCertificate_CertificateNotFound()
        {
            // arrange 
            var submitCertificate = new SubmitCertificateRequest { Uln = 9876543210, FamilyName = "Blogs", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Certificate not found" };

            var expectedResponse = new List<SubmitCertificateResponse>
            {
                new SubmitCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificateRequest> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task SubmitCertificate_CertificateStatusInvalid()
        {
            // arrange 
            var submitCertificate = new SubmitCertificateRequest { Uln = 1234567890, FamilyName = "Blogs", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Certificate is not in 'Ready' status" };

            var expectedResponse = new List<SubmitCertificateResponse>
            {
                new SubmitCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificateRequest> { submitCertificate });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().Certificate, Is.Null);
        }

        [Test]
        public async Task SubmitCertificate_InvalidFamilyName()
        {
            // arrange 
            var submitCertificate = new SubmitCertificateRequest { Uln = 1234567890, FamilyName = "INVALID", StandardCode = 1 };

            var expectedValidationErrors = new List<string> { "Cannot find apprentice with the specified Uln, FamilyName & Standard" };

            var expectedResponse = new List<SubmitCertificateResponse>
            {
                new SubmitCertificateResponse { Certificate = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/certificate/submit")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.SubmitCertificates(new List<SubmitCertificateRequest> { submitCertificate });

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
            string standard = "1";
            string certificateReference = "123456790";

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/api/v1/certificate/{uln}/{lastname}/{standard}/{certificateReference}")
                .Respond(HttpStatusCode.OK, "application/json", string.Empty);

            // act
            var request = new DeleteCertificateRequest { Uln = uln, FamilyName = lastname, Standard = standard, CertificateReference = certificateReference };
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
            string standard = "4321";
            string certificateReference = "1234567890";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Certificate not found"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/api/v1/certificate/{uln}/{lastname}/{standard}/{certificateReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteCertificateRequest { Uln = uln, FamilyName = lastname, Standard = standard, CertificateReference = certificateReference };
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
            string standard = "1";
            string certificateReference = "1234567890";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Cannot delete a submitted Certificate"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/api/v1/certificate/{uln}/{lastname}/{standard}/{certificateReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteCertificateRequest { Uln = uln, FamilyName = lastname, Standard = standard, CertificateReference = certificateReference };
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
            string standard = "1";
            string certificateReference = "1234567890";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Cannot find apprentice with the specified Uln, FamilyName & Standard"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/api/v1/certificate/{uln}/{lastname}/{standard}/{certificateReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteCertificateRequest { Uln = uln, FamilyName = lastname, Standard = standard, CertificateReference = certificateReference };
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
            string standard = "1";
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

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/certificate/{uln}/{lastname}/{standard}")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new GetCertificateRequest { Uln = uln, FamilyName = lastname, Standard = standard };
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
            string standard = "4321";

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/certificate/{uln}/{lastname}/{standard}")
                .Respond(HttpStatusCode.NoContent, "application/json", string.Empty);

            // act
            var request = new GetCertificateRequest { Uln = uln, FamilyName = lastname, Standard = standard };
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
            string standard = "4321";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Cannot find apprentice with the specified Uln, FamilyName & Standard"
            };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/certificate/{uln}/{lastname}/{standard}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new GetCertificateRequest { Uln = uln, FamilyName = lastname, Standard = standard };
            var actual = await _ApiClient.GetCertificate(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
            Assert.That(actual.Certificate, Is.Null);
        }

        [Test]
        public async Task GetGrades()
        {
            // arrange 
            var expectedResponse = new List<string>
            {
                "Pass", "Credit", "Merit", "Distinction", "Pass with excellence", "No grade awarded"
            };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/api/v1/certificate/grades")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.GetGrades();

            // assert
            Assert.That(actual, Has.Count.EqualTo(6));
            Assert.That(actual, Contains.Item("Distinction"));
        }

    }
}
