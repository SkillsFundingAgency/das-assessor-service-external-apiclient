namespace SFA.DAS.AssessorService.ExternalApi.Examples
{
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main(string[] args)
        {
            const string subscriptionKey = ""; // insert your key here
            const string apiBaseAddress = ""; // insert the API address here
            
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            httpClient.BaseAddress = new Uri(apiBaseAddress);

            CertificateApiClient certificateApiClient = new CertificateApiClient(httpClient);
            SearchApiClient searchApiClient = new SearchApiClient(httpClient);

            Program p = new Program(certificateApiClient, searchApiClient);
        }


        private readonly CertificateApiClient _CertificateApiClient;
        private readonly SearchApiClient _SearchApiClient;


        public Program(CertificateApiClient certificateApiClient, SearchApiClient searchApiClient)
        {
            _CertificateApiClient = certificateApiClient;
            _SearchApiClient = searchApiClient;
        }

        public async Task SearchExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            int? standardCode = null;

            await _SearchApiClient.Search(uln, lastName, standardCode);
        }

        public async Task CreateCertificatesExample()
        {
            long uln = 1234567890;
            string firstName = "Fred";
            string lastName = "Blogs";
            int standardCode = 1;
            string overallGrade = "PASS";
            string contactName = "Shreya Smith";
            string organisation = "Contoso Ltd";
            string address = "123 Test Road";
            string city = "Townsville";
            string postcode = "ZY9 9ZY";

            CertificateData newCertificate = new CertificateData
            {
                CertificateReference = null,
                Learner = new Learner { Uln = uln, GivenNames = firstName, FamilyName = lastName },
                LearningDetails = new LearningDetails { StandardCode = standardCode, OverallGrade = overallGrade, AchievementDate = DateTime.UtcNow },
                PostalContact = new PostalContact { ContactName = contactName, Organisation = organisation, AddressLine1 = address, City = city, PostCode = postcode }
            };

            if (newCertificate.IsValid(out ICollection<ValidationResult> validationResults))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.CreateCertificates(new List<CertificateData> { newCertificate });
            }
        }

        public async Task UpdateCertificatesExample()
        {
            // NOTE: You will need to know what the certificate currently looks like
            Certificate currentCertificate = new Certificate
            {
                Status = "Draft",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Example",
                CertificateData = new CertificateData
                {
                    CertificateReference = "1234567890-1",
                    Learner = new Learner { Uln = 1234567890, GivenNames = "Fred", FamilyName = "Bloggs" },
                    LearningDetails = new LearningDetails
                    {
                        StandardCode = 1, StandardLevel = 1, StandardName = "Example Standard", LearningStartDate = DateTime.UtcNow.AddYears(-1),
                        ProviderName = "Test Provider", ProviderUkPrn = 123456, OverallGrade = "Pass", AchievementDate = DateTime.UtcNow
                    },
                    PostalContact = new PostalContact { ContactName = "Shreya Smith", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", City = "Townsville", PostCode = "ZY99ZZ" }
                }
            };

            // Let's pretend the apprentice got a better grade
            CertificateData updatedCertificate = currentCertificate.CertificateData;
            updatedCertificate.LearningDetails.OverallGrade = "Merit";

            if (updatedCertificate.IsValid(out ICollection<ValidationResult> validationResults))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.UpdateCertificates(new List<CertificateData> { updatedCertificate });
            }
        }

        public async Task SubmitCertificatesExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            int standardCode = 1;

            SubmitCertificate certificateToSubmit = new SubmitCertificate
            {
                Uln = uln,
                FamilyName = lastName,
                StandardCode = standardCode
            };

            if (certificateToSubmit.IsValid(out ICollection<ValidationResult> validationResults))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.SubmitCertificates(new List<SubmitCertificate> { certificateToSubmit });
            }
        }

        public async Task DeleteCertificateExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            int standardCode = 1;

            await _CertificateApiClient.DeleteCertificate(uln, lastName, standardCode);
        }
    }
}
