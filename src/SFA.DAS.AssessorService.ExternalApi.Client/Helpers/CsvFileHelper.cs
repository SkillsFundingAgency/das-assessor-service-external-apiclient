namespace SFA.DAS.AssessorService.ExternalApi.Client.Helpers
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class CsvFileHelper<T>
    {
        public static IEnumerable<T> GetFromFile(string filePath, ClassMap<T> map = null)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using (TextReader textReader = new StreamReader(stream))
                {
                    using (CsvReader csv = new CsvReader(textReader))
                    {
                        csv.Configuration.HeaderValidated = null;
                        csv.Configuration.MissingFieldFound = null;
                        csv.Configuration.BadDataFound = null;
                        csv.Configuration.ReadingExceptionOccurred = null;

                        if (map != null)
                        {
                            csv.Configuration.RegisterClassMap(map);
                        }

                        return csv.GetRecords<T>().ToList();
                    }
                }
            }
            catch (SystemException)
            {
                return new List<T>();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        public static void SaveToFile(string filePath, IEnumerable<T> records)
        {
            try
            {
                using (TextWriter textReader = File.CreateText(filePath))
                {
                    using (CsvWriter csv = new CsvWriter(textReader))
                    {
                        csv.WriteHeader<T>();
                        csv.NextRecord();
                        csv.WriteRecords(records);
                        csv.NextRecord();
                    }
                }
            }
            catch (SystemException)
            {
                // ignore
            }
        }
    }
}
