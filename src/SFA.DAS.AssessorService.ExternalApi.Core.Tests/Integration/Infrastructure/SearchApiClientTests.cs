namespace SFA.DAS.AssessorService.ExternalApi.Core.Tests.Integration.Infrastructure
{
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Search;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    [TestFixture(Category = "Infrastructure")]
    public sealed class SearchApiClientTests : IDisposable
    {
        private const string apiBaseAddress = "http://localhost";
        private const string subscriptionKey = "test";

        private SearchApiClient _ApiClient;
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

            _ApiClient = new SearchApiClient(httpClient);
        }

        [TearDown]
        public void Dispose()
        {
            _MockHttp?.Dispose();
        }

        [Test]
        public async Task SearchWithNoFilter()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "test";

            var expected = new List<SearchResult>
            {
                new SearchResult { Uln = 1234567890, FamilyName = "test", StdCode = 1234, UkPrn = 12345678 },
                new SearchResult { Uln = 1234567890, FamilyName = "test", StdCode = 4321, UkPrn = 87654321 },
                new SearchResult { Uln = 1234567890, FamilyName = "test", StdCode = 9999, UkPrn = 12345678 }
            };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/learner/{uln}/{lastname}/")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expected));

            // act
            var actual = await _ApiClient.Search(uln, lastname);

            // assert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public async Task SearchWithFilter()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "test";
            int standardcode = 1234;

            var expected = new List<SearchResult>
            {
                new SearchResult { Uln = 1234567890, FamilyName = "test", StdCode = 1234, UkPrn = 12345678 },
            };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/learner/{uln}/{lastname}/{standardcode}")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expected));

            // act
            var actual = await _ApiClient.Search(uln, lastname, standardcode);

            // assert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public async Task SearchWithNoMatchingResults()
        {
            // arrange 
            long uln = 1234567890;
            string lastname = "test";
            int standardcode = 2233;

            var expected = new List<SearchResult>
            {
            };

            _MockHttp.When(HttpMethod.Get, $"{apiBaseAddress}/learner/{uln}/{lastname}/{standardcode}")
                .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(expected));

            // act
            var actual = await _ApiClient.Search(uln, lastname, standardcode);

            // assert
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
