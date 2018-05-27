using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

namespace Payslip.DataAccess
{
    public sealed class CsvEmployeeRecordRepository
    {
        public IEnumerable<Record> ReadRecordsFromStream(FileStream fileStream)
        {
            IEnumerable<Record> result; 
            using (var streamReader = new StreamReader(fileStream))
            using (var csvReader = new CsvReader(streamReader))
            {
                csvReader.Configuration.PrepareHeaderForMatch = header => header.Replace("_", string.Empty).ToLowerInvariant();
                result = csvReader.GetRecords<Record>().ToArray();
            }
            
            return result;
        }

        /**
         *  using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream))
        using (var csvWriter = new CsvWriter(streamWriter))
         */
    }
}