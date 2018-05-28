using System.Collections.Generic;
using System.IO;
using Payslip.Model;

namespace Payslip.DataAccess
{
    public interface IEmployeeRepository
    {
        IEnumerable<T> ReadRecordsFromStream<T>(Stream stream);
        byte[] WriteRecordsToBytes<T>(IEnumerable<T> records);
    }
}