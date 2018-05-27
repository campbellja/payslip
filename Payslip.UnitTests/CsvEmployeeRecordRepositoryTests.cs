using System.IO;
using System.Linq;
using Payslip.DataAccess;
using Shouldly;
using Xunit;
using Record = Payslip.DataAccess.Record;

namespace Payslip.UnitTests
{
    public sealed class CsvEmployeeRecordRepositoryTests
    {
        private static CsvEmployeeRecordRepository BuildCsvEmployeeRecordRepository()
        {
            return new CsvEmployeeRecordRepository();
        }

        [Fact]
        public void ReadRecords_ValidCsvFile_ReturnsRecords()
        {
            // arrange
            const string filePath = @"C:\source\github\margherita-pizza\doc\input.csv";
            
            var expected = new[]
            {
                new Record
                {
                    FirstName="David",
                    LastName="Rudd",
                    AnnualSalary="60050",
                    SuperRate="9%",
                    PaymentStartDate = "01 March - 31 March"
                },
                new Record
                {
                    FirstName="Ryan",
                    LastName="Chen",
                    AnnualSalary="120000",
                    SuperRate="10%",
                    PaymentStartDate = "01 March - 31 March"
                }
            };

            // act
            Record[] records;
            using (var fileStream = File.OpenRead(filePath))
            {
                records = BuildCsvEmployeeRecordRepository().ReadRecordsFromStream(fileStream).ToArray();
            }

            // assert
            for (var i = 0; i < expected.Length; i++)
            {
                records[i].FirstName.ShouldBe(expected[i].FirstName);
                records[i].LastName.ShouldBe(expected[i].LastName);
                records[i].AnnualSalary.ShouldBe(expected[i].AnnualSalary);
                records[i].SuperRate.ShouldBe(expected[i].SuperRate);
                records[i].PaymentStartDate.ShouldBe(expected[i].PaymentStartDate);
            }
        }

        [Fact]
        public void WriteRecordToStream()
        {
            
            //BuildCsvEmployeeRecordRepository().ReadRecordsToBytes()
        }
    }
}