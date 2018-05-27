using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Payslip.DataAccess;
using Payslip.Model;
using Shouldly;
using Xunit;
using Record = Payslip.DataAccess.Record;

namespace Payslip.UnitTests
{
    public sealed class CsvPayslipRepositoryTests
    {
        private static CsvPayslipRepository BuildCsvEmployeeRecordRepository()
        {
            return new CsvPayslipRepository();
        }

        [Category("Integration")]
        [Fact]
        public void ReadRecordsFromStream_CsvFileStream_ReturnsRecords()
        {
            // arrange
            var inputCsvFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"TestData\input.csv");
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
            using (var fileStream = File.OpenRead(inputCsvFilePath))
            {
                records = BuildCsvEmployeeRecordRepository().ReadRecordsFromStream<Record>(fileStream).ToArray();
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

        [Category("Integration")]
        [Fact]
        public void ReadRecordsFromStream_ClosedStream_ThrowsArgumentException()
        {
            // arrange
            var stream = new FileStream(Path.GetTempFileName(), FileMode.Open);
            stream.Close();
            // act & assert
            Assert.Throws<ArgumentException>(() => BuildCsvEmployeeRecordRepository().ReadRecordsFromStream<object>(stream));
        }

        [Fact]
        public void WriteRecordsToBytes_Payslips_WriteCsvLinesToByteArray()
        {
            // arrange
            const decimal @decimal = 12345678M;
            var payslips = new[]
            {
                new EmployeePayslip
                {
                    Name = BuildRandomString(),
                    PayPeriod = BuildRandomString(),
                    GrossIncome = @decimal,
                    IncomeTax = @decimal,
                    NetIncome = @decimal,
                    Super = @decimal
                }
            };
            // act
            var bytes = BuildCsvEmployeeRecordRepository().WriteRecordsToBytes(payslips);
            // assert
            using (var stream = new StreamReader(new MemoryStream(bytes)))
            {
                Assert_FirstLineIsHeaderRow(stream);
                foreach (var p in payslips)
                {
                    var line = stream.ReadLine();
                    line.ShouldContain($"{p.Name},{p.PayPeriod},{p.GrossIncome},{p.IncomeTax},{p.NetIncome},{p.Super}");
                }
            } 
        }

        private static void Assert_FirstLineIsHeaderRow(StreamReader stream)
        {
            var actualHeader = stream.ReadLine();
            actualHeader.ShouldContain("Name,PayPeriod,GrossIncome,IncomeTax,NetIncome,Super");
        }

        [Fact]
        public void WriteRecordsToBytes_EmptyPayslipArray_ReturnsHeaderRowOnly()
        {
            // arrange & act
            var bytes = BuildCsvEmployeeRecordRepository().WriteRecordsToBytes(new EmployeePayslip[0]);
            // assert
            using (var stream = new StreamReader(new MemoryStream(bytes)))
            {
                Assert_FirstLineIsHeaderRow(stream);
                stream.ReadLine().ShouldBeNull();
            }
        }

        private string BuildRandomString()
        {
            return Guid.NewGuid() + "";
        }
    }
}