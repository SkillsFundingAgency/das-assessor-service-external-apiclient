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

            Program p = new Program(certificateApiClient);
            p.CreateCertificatesExample().GetAwaiter().GetResult();
            p.UpdateCertificatesExample().GetAwaiter().GetResult();
            p.SubmitCertificatesExample().GetAwaiter().GetResult();
            p.DeleteCertificateExample().GetAwaiter().GetResult();
            p.GetCertificateExample().GetAwaiter().GetResult();
            p.GetGradesExample().GetAwaiter().GetResult();
        }


        private readonly CertificateApiClient _CertificateApiClient;

        public Program(CertificateApiClient certificateApiClient)
        {
            _CertificateApiClient = certificateApiClient;
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

            CreateCertificate newCertificate = new CreateCertificate
            {
                Learner = new Learner { Uln = uln, GivenNames = firstName, FamilyName = lastName },
                Standard = new Standard { StandardCode = standardCode },
                LearningDetails = new LearningDetails { OverallGrade = overallGrade, AchievementDate = DateTime.UtcNow },
                PostalContact = new PostalContact { ContactName = contactName, Organisation = organisation, AddressLine1 = address, City = city, PostCode = postcode }
            };

            if (newCertificate.IsValid(out ICollection<ValidationResult> validationResults))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.CreateCertificates(new List<CreateCertificate> { newCertificate });
            }
        }

        public async Task UpdateCertificatesExample()
        {
            // NOTE: You will need to know what the certificate currently looks like
            Certificate currentCertificate = new Certificate
            {
                Status = new Status { CurrentStatus = "Draft" },
                Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Example" },
                CertificateData = new CertificateData
                {
                    CertificateReference = "00012001",
                    Learner = new Learner { Uln = 1234567890, GivenNames = "Fred", FamilyName = "Bloggs" },
                    Standard = new Standard { StandardCode = 1, Level = 1, StandardName = "Example Standard", },
                    LearningDetails = new LearningDetails
                    {
                        LearningStartDate = DateTime.UtcNow.AddYears(-1),
                        ProviderName = "Test Provider",
                        ProviderUkPrn = 123456,
                        OverallGrade = "Pass",
                        AchievementDate = DateTime.UtcNow
                    },
                    PostalContact = new PostalContact { ContactName = "Shreya Smith", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", City = "Townsville", PostCode = "ZY99ZZ" }
                }
            };

            // Let's pretend the apprentice got a better grade
            UpdateCertificate updatedCertificate = new UpdateCertificate
            {
                CertificateReference = currentCertificate.CertificateData.CertificateReference,
                Learner = currentCertificate.CertificateData.Learner,
                Standard = currentCertificate.CertificateData.Standard,
                LearningDetails = currentCertificate.CertificateData.LearningDetails,
                PostalContact = currentCertificate.CertificateData.PostalContact,
            };

            updatedCertificate.LearningDetails.OverallGrade = "Merit";

            if (updatedCertificate.IsValid(out ICollection<ValidationResult> validationResults))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.UpdateCertificates(new List<UpdateCertificate> { updatedCertificate });
            }
        }

        public async Task SubmitCertificatesExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            int standardCode = 1;
            string certificateReference = "00012001";

            SubmitCertificate certificateToSubmit = new SubmitCertificate
            {
                Uln = uln,
                FamilyName = lastName,
                StandardCode = standardCode,
                CertificateReference = certificateReference
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
            string standard = "1";
            string certificateReference = "00012001";

            DeleteCertificate certificateToDelete = new DeleteCertificate
            {
                Uln = uln,
                FamilyName = lastName,
                Standard = standard,
                CertificateReference = certificateReference
            };

            if (certificateToDelete.IsValid(out ICollection<ValidationResult> validationResults))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.DeleteCertificate(certificateToDelete);
            }
        }

        public async Task GetCertificateExample()
        {
            long uln = 1234567890;
            string lastName = "Blogs";
            string standard = "1";

            GetCertificate certificateToGet = new GetCertificate
            {
                Uln = uln,
                FamilyName = lastName,
                Standard = standard,
            };

            if (certificateToGet.IsValid(out ICollection<ValidationResult> validationResults))
            {
                // NOTE: The External API performs validation, however it is a good idea to check beforehand
                await _CertificateApiClient.GetCertificate(certificateToGet);
            }
        }

        public async Task GetGradesExample()
        {
            await _CertificateApiClient.GetGrades();
        }
    }
}
