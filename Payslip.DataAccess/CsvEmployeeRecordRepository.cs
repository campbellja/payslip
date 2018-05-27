using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Payslip.Model;

namespace Payslip.DataAccess
{
    public sealed class CsvEmployeeRecordRepository
    {
        public IEnumerable<Record> ReadRecordsFromStream(Stream stream)
        {
            IEnumerable<Record> result; 
            using (var streamReader = new StreamReader(stream))
            using (var csvReader = new CsvReader(streamReader))
            {
                csvReader.Configuration.PrepareHeaderForMatch = header => header.Replace("_", string.Empty).ToLowerInvariant();
                result = csvReader.GetRecords<Record>().ToArray();
            }
            
            return result;
        }

        public byte[] WriteRecordsToBytes(IEnumerable<EmployeePayslip> payslips)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.WriteRecords(payslips);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}