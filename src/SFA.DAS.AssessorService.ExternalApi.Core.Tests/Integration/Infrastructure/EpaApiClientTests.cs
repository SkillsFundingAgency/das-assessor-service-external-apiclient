namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Integration.Infrastructure
{
    using FizzWare.NBuilder;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Error;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Request.Epa;
    using SFA.DAS.AssessorService.ExternalApi.Core.Messages.Response.Epa;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Epa;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    [TestFixture(Category = "Infrastructure")]
    public sealed class EpaApiClientTests : IDisposable
    {
        private const string apiBaseAddress = "http://localhost";
        private const string subscriptionKey = "test";

        private EpaApiClient _ApiClient;
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

            _ApiClient = new EpaApiClient(httpClient);
        }

        [TearDown]
        public void Dispose()
        {
            _MockHttp?.Dispose();
        }

        [Test]
        public async Task CreateEpaRecord()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().Build();
            var epaRecord = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var epaRequest = new CreateEpaRequest
            {
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedResponse = new List<CreateEpaResponse>
            {
                new CreateEpaResponse { EpaReference = "1234567890" }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateEpaRecords(new List<CreateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().EpaReference, Is.EqualTo(expectedResponse.First().EpaReference));
        }

        [Test]
        public async Task CreateEpaRecord_EpaExists()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().Build();
            var epaRecord = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var epaRequest = new CreateEpaRequest
            {
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedValidationErrors = new List<string> { "EPA already provided for the learner" };

            var expectedResponse = new List<CreateEpaResponse>
            {
                new CreateEpaResponse { EpaReference = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateEpaRecords(new List<CreateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().EpaReference, Is.Null);
        }

        public async Task CreateEpaRecord_CertificateExists()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().Build();
            var epaRecord = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var epaRequest = new CreateEpaRequest
            {
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedValidationErrors = new List<string> { "Certificate already exists, cannot create EPA record" };

            var expectedResponse = new List<CreateEpaResponse>
            {
                new CreateEpaResponse { EpaReference = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateEpaRecords(new List<CreateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().EpaReference, Is.Null);
        }

        [Test]
        public async Task CreateEpaRecord_InvalidEpaOutcome()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().Build();
            var epaRecord = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "INVALID").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var epaRequest = new CreateEpaRequest
            {
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedValidationErrors = new List<string> { "Invalid outcome: must be Pass, Fail or Withdrawn" };

            var expectedResponse = new List<CreateEpaResponse>
            {
                new CreateEpaResponse { EpaReference = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateEpaRecords(new List<CreateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().EpaReference, Is.Null);
        }

        [Test]
        public async Task CreateEpaRecord_Using_StandardCode_NotFound()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().With(s => s.StandardCode = 5555).Build();
            var epaRecord = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var epaRequest = new CreateEpaRequest
            {
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedValidationErrors = new List<string> { "Unable to find specified Standard" };

            var expectedResponse = new List<CreateEpaResponse>
            {
                new CreateEpaResponse { EpaReference = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateEpaRecords(new List<CreateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().EpaReference, Is.Null);
        }

        [Test]
        public async Task CreateEpaRecord_Using_StandardReference_NotFound()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().With(s => s.StandardReference = "INVALID").Build();
            var epaRecord = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord }).Build();

            var epaRequest = new CreateEpaRequest
            {
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedValidationErrors = new List<string> { "Unable to find specified Standard" };

            var expectedResponse = new List<CreateEpaResponse>
            {
                new CreateEpaResponse { EpaReference = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Post, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.CreateEpaRecords(new List<CreateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().EpaReference, Is.Null);
        }

        [Test]
        public async Task UpdateEpaRecord()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().Build();
            var epaRecord1 = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Fail").With(er => er.EpaDate = DateTime.Now.AddDays(-2)).Build();
            var epaRecord2 = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").With(er => er.EpaDate = DateTime.Now).Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord1, epaRecord2 }).Build();

            var epaRequest = new UpdateEpaRequest
            {
                EpaReference = "1234567890",
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedResponse = new List<UpdateEpaResponse>
            {
                new UpdateEpaResponse { EpaReference = "1234567890" }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateEpaRecords(new List<UpdateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Has.Count.EqualTo(0));
            Assert.That(actual.First().EpaReference, Is.EqualTo(expectedResponse.First().EpaReference));
        }

        [Test]
        public async Task UpdateEpaRecord_EpaRecordNotFound()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().Build();
            var epaRecord1 = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Fail").With(er => er.EpaDate = DateTime.Now.AddDays(-2)).Build();
            var epaRecord2 = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").With(er => er.EpaDate = DateTime.Now).Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord1, epaRecord2 }).Build();

            var epaRequest = new UpdateEpaRequest
            {
                EpaReference = "NOT FOUND",
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedValidationErrors = new List<string> { "EPA not found" };

            var expectedResponse = new List<UpdateEpaResponse>
            {
                new UpdateEpaResponse { EpaReference = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateEpaRecords(new List<UpdateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().EpaReference, Is.Null);
        }

        [Test]
        public async Task UpdateEpaRecord_CertificateExists()
        {
            // arrange 
            var learner = Builder<Models.Certificates.Learner>.CreateNew().Build();
            var standard = Builder<Models.Certificates.Standard>.CreateNew().Build();
            var epaRecord1 = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Fail").With(er => er.EpaDate = DateTime.Now.AddDays(-2)).Build();
            var epaRecord2 = Builder<EpaRecord>.CreateNew().With(er => er.EpaOutcome = "Pass").With(er => er.EpaDate = DateTime.Now).Build();
            var epaDetails = Builder<EpaDetails>.CreateNew().With(ed => ed.Epas = new List<EpaRecord> { epaRecord1, epaRecord2 }).Build();

            var epaRequest = new UpdateEpaRequest
            {
                EpaReference = "CERTIFICATE EXISTS",
                Learner = learner,
                Standard = standard,
                EpaDetails = epaDetails
            };

            var expectedValidationErrors = new List<string> { "Certificate already exists, cannot update EPA record" };

            var expectedResponse = new List<UpdateEpaResponse>
            {
                new UpdateEpaResponse { EpaReference = null, ValidationErrors = expectedValidationErrors }
            };

            _MockHttp.When(HttpMethod.Put, $"{apiBaseAddress}/api/v1/epa")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var actual = await _ApiClient.UpdateEpaRecords(new List<UpdateEpaRequest> { epaRequest });

            // assert
            Assert.That(actual, Has.Count.EqualTo(1));
            Assert.That(actual.First().ValidationErrors, Is.EqualTo(expectedResponse.First().ValidationErrors));
            Assert.That(actual.First().EpaReference, Is.Null);
        }

        [Test]
        public async Task DeleteEpaRecord()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            string standard = "1";
            string epaReference = "123456790";

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/api/v1/epa/{uln}/{lastname}/{standard}/{epaReference}")
                .Respond(HttpStatusCode.OK, "application/json", string.Empty);

            // act
            var request = new DeleteEpaRequest { Uln = uln, FamilyName = lastname, Standard = standard, EpaReference = epaReference };
            var actual = await _ApiClient.DeleteEpaRecord(request);

            // assert
            Assert.That(actual.Error, Is.Null);
        }

        [Test]
        public async Task DeleteEpaRecord_EpaRecordNotFound()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            string standard = "4321";
            string epaReference = "1234567890";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Epa not found"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/api/v1/epa/{uln}/{lastname}/{standard}/{epaReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteEpaRequest { Uln = uln, FamilyName = lastname, Standard = standard, EpaReference = epaReference };
            var actual = await _ApiClient.DeleteEpaRecord(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
        }

        [Test]
        public async Task DeleteEpaRecord_CertificateExists()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "Bloggs";
            string standard = "1";
            string epaReference = "CERTIFICATE_EXISTS";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Certificate already exists, cannot delete EPA record"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/api/v1/epa/{uln}/{lastname}/{standard}/{epaReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteEpaRequest { Uln = uln, FamilyName = lastname, Standard = standard, EpaReference = epaReference };
            var actual = await _ApiClient.DeleteEpaRecord(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
        }

        [Test]
        public async Task DeleteEpaRecord_InvalidFamilyName()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "INVALID";
            string standard = "1";
            string epaReference = "1234567890";

            var expectedResponse = new ApiResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Cannot find apprentice with the specified Uln, FamilyName & Standard"
            };

            _MockHttp.When(HttpMethod.Delete, $"{apiBaseAddress}/api/v1/epa/{uln}/{lastname}/{standard}/{epaReference}")
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // act
            var request = new DeleteEpaRequest { Uln = uln, FamilyName = lastname, Standard = standard, EpaReference = epaReference };
            var actual = await _ApiClient.DeleteEpaRecord(request);

            // assert
            Assert.That(actual.Error, Is.Not.Null);
            Assert.That(actual.Error.StatusCode, Is.EqualTo(expectedResponse.StatusCode));
            Assert.That(actual.Error.Message, Is.EqualTo(expectedResponse.Message));
        }
    }
}
