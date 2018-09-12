namespace SFA.DAS.AssessorService.ExternalApi.Client.Helpers
{
    using CsvHelper;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class CsvFileHelper<T>
    {
        public static IEnumerable<T> GetFromFile(string filePath)
        {
            try
            {
                using (TextReader textReader = File.OpenText(filePath))
                {
                    CsvReader csv = new CsvReader(textReader);
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;

                    return csv.GetRecords<T>().ToList();
                }
            }
            catch(SystemException)
            {
                return null;
            }
        }

        public static void SaveToFile(string filePath, IEnumerable<T> records)
        {
            try
            {
                using (TextWriter textReader = File.CreateText(filePath))
                {
                    CsvWriter csv = new CsvWriter(textReader);

                    csv.WriteHeader<T>();
                    csv.NextRecord();
                    csv.WriteRecords(records);
                    csv.NextRecord();
                }
            }
            catch (SystemException)
            {
                // ignore
            }
        }
    }
}
