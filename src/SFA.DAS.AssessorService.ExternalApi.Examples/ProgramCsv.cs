namespace SFA.DAS.AssessorService.ExternalApi.Examples
{
    using CsvHelper;
    using SFA.DAS.AssessorService.ExternalApi.Core.Infrastructure;
    using SFA.DAS.AssessorService.ExternalApi.Core.Models.Certificates;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ProgramCsv
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

            ProgramCsv p = new ProgramCsv(certificateApiClient, searchApiClient);
            p.CreateCertificatesExample().GetAwaiter().GetResult();
            p.UpdateCertificatesExample().GetAwaiter().GetResult();
            p.SubmitCertificatesExample().GetAwaiter().GetResult();
            p.DeleteCertificatesExample().GetAwaiter().GetResult();
        }


        private readonly CertificateApiClient _CertificateApiClient;
        private readonly SearchApiClient _SearchApiClient;


        public ProgramCsv(CertificateApiClient certificateApiClient, SearchApiClient searchApiClient)
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
            const string filePath = @"CsvFiles\createCertificates.csv";

            IEnumerable<CertificateData> certificatesToCreate;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificatesToCreate = csv.GetRecords<CertificateData>().ToList();
            }

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificatesToCreate.Any(c => !c.IsValid(out ICollection<ValidationResult> validationResults));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                var response = (await _CertificateApiClient.CreateCertificates(certificatesToCreate)).ToList();

                // NOTE: You may want to deal with good & bad records seperately
                var goodCertificates = response.Where(c => c.Certificate != null && !c.ValidationErrors.Any());
                var badCertificates = response.Except(goodCertificates);
            }
        }

        public async Task UpdateCertificatesExample()
        {
            const string filePath = @"CsvFiles\updateCertificates.csv";

            IEnumerable<Certificate> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificates = csv.GetRecords<Certificate>().ToList();
            }

            IEnumerable<CertificateData> certificatesToUpdate = certificates.Select(c => c.CertificateData);

            // Let's pretend the first and last apprentices got better grades
            certificatesToUpdate.First().LearningDetails.AchievementOutcome = "MERIT";
            certificatesToUpdate.Last().LearningDetails.AchievementOutcome = "PASS";

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificatesToUpdate.Any(c => !c.IsValid(out ICollection<ValidationResult> validationResults));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                var response = (await _CertificateApiClient.UpdateCertificates(certificatesToUpdate)).ToList();

                // NOTE: You may want to deal with good & bad records seperately
                var goodCertificates = response.Where(c => c.Certificate != null && !c.ValidationErrors.Any());
                var badCertificates = response.Except(goodCertificates);
            }

        }

        public async Task SubmitCertificatesExample()
        {
            const string filePath = @"CsvFiles\submitCertificates.csv";

            IEnumerable<Certificate> certificates;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificates = csv.GetRecords<Certificate>().ToList();
            }

            IEnumerable<SubmitCertificate> certificatesToSubmit = certificates.Select(c => new SubmitCertificate { Uln = c.CertificateData.Learner.Uln, FamilyName = c.CertificateData.Learner.FamilyName, StandardCode = c.CertificateData.LearningDetails.StandardCode});

            // NOTE: The External API performs validation, however it is a good idea to check beforehand.
            bool invalidDataSupplied = certificatesToSubmit.Any(c => !c.IsValid(out ICollection<ValidationResult> validationResults));

            if (invalidDataSupplied)
            {
                throw new InvalidOperationException("The supplied CSV file contains invalid data. Please correct and then try again.");
            }
            else
            {
                var response = (await _CertificateApiClient.SubmitCertificates(certificatesToSubmit)).ToList();

                // NOTE: You may want to deal with good & bad records seperately
                var goodCertificates = response.Where(c => c.Certificate != null && !c.ValidationErrors.Any());
                var badCertificates = response.Except(goodCertificates);
            }
        }

        public async Task DeleteCertificatesExample()
        {
            const string filePath = @"CsvFiles\deleteCertificates.csv";

            IEnumerable<Certificate> certificatesToDelete;

            using (TextReader textReader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(textReader);
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                certificatesToDelete = csv.GetRecords<Certificate>().ToList();
            }

            // NOTE: The External API does not have an batch delete (for safety reasons). You'll have to loop.
            foreach(var certificate in certificatesToDelete)
            {
                var response = await _CertificateApiClient.DeleteCertificate(certificate.CertificateData.Learner.Uln, certificate.CertificateData.Learner.FamilyName, certificate.CertificateData.LearningDetails.StandardCode);

                if(!string.IsNullOrEmpty(Convert.ToString(response)))
                {
                    // NOTE: You may want to deal with bad records seperately
                }
            }
        }
    }
}
