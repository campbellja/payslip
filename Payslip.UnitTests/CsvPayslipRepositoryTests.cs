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
            var inputCsvFile = CsvFile();
            var expectedRecords = new[]
            {
                new Record
                {
                    FirstName="David",
                    LastName="Rudd",
                    AnnualSalary= 60050M,
                    SuperRate=0.09M,
                    PaymentStartDate = "01 March - 31 March"
                },
                new Record
                {
                    FirstName="Ryan",
                    LastName="Chen",
                    AnnualSalary=120000M,
                    SuperRate=0.1M,
                    PaymentStartDate = "01 March - 31 March"
                }
            };

            // act
            Record[] records;
            using (var fileStream = inputCsvFile)
            {
                records = BuildCsvEmployeeRecordRepository().ReadRecordsFromStream<Record>(fileStream).ToArray();
            }

            // assert
            for (var i = 0; i < expectedRecords.Length; i++)
            {
                records[i].FirstName.ShouldBe(expectedRecords[i].FirstName);
                records[i].LastName.ShouldBe(expectedRecords[i].LastName);
                records[i].AnnualSalary.ShouldBe(expectedRecords[i].AnnualSalary);
                records[i].SuperRate.ShouldBe(expectedRecords[i].SuperRate);
                records[i].PaymentStartDate.ShouldBe(expectedRecords[i].PaymentStartDate);
            }
        }

        [Category("Integration")]
        [Fact]
        public void ReadRecordsFromStream_ReadFileStream_DisposesStream()
        {
            // arrange & act
            using (var fileStream = CsvFile())
            {
                BuildCsvEmployeeRecordRepository().ReadRecordsFromStream<Record>(fileStream);
                // assert
                Assert.Throws<ObjectDisposedException>(() => fileStream.ReadByte());
            }
        }

        private static FileStream CsvFile()
        {
            return File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), @"TestData\input.csv"));
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