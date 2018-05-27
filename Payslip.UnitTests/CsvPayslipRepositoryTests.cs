using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Payslip.DataAccess;
using Payslip.Model;
using Shouldly;
using Xunit;

namespace Payslip.UnitTests
{
    public sealed class CsvPayslipRepositoryTests
    {
        private static FileStream CsvFile()
        {
            return File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), @"TestData\input.csv"));
        }

        private static CsvPayslipRepository BuildCsvEmployeeRecordRepository()
        {
            return new CsvPayslipRepository();
        }

        [Category("Integration")]
        [Fact]
        public void ReadRecordsFromStream_CsvFileStream_ReturnsRecords()
        {
            // arrange
            var currentYear = DateTime.Today.Year;
            var inputCsvFile = CsvFile();
            var expectedRecords = new[]
            {
                new Employee
                {
                    FirstName="David",
                    LastName="Rudd",
                    AnnualSalary= 60050M,
                    SuperRate=0.09M,
                    PaymentStartDate = new PaymentPeriod(new DateTime(currentYear, 3, 1),new DateTime(currentYear, 3, 31))
                },
                new Employee
                {
                    FirstName="Ryan",
                    LastName="Chen",
                    AnnualSalary=120000M,
                    SuperRate=0.1M,
                    PaymentStartDate = new PaymentPeriod(new DateTime(currentYear, 3, 1),new DateTime(currentYear, 3, 31))
                },
                new Employee
                {
                    FirstName="Alison",
                    LastName="Harvey",
                    AnnualSalary=125000M,
                    SuperRate=0.1M,
                    PaymentStartDate = new PaymentPeriod(new DateTime(currentYear, 6, 1),new DateTime(currentYear, 6, 30))
                }
            };
            
             // act
             Employee[] actualEmployees;
            using (var fileStream = inputCsvFile)
            {
                actualEmployees = BuildCsvEmployeeRecordRepository().ReadRecordsFromStream<Employee>(fileStream).ToArray();
            }

            // assert
            for (var i = 0; i < expectedRecords.Length; i++)
            {
                actualEmployees[i].FirstName.ShouldBe(expectedRecords[i].FirstName);
                actualEmployees[i].LastName.ShouldBe(expectedRecords[i].LastName);
                actualEmployees[i].AnnualSalary.ShouldBe(expectedRecords[i].AnnualSalary);
                actualEmployees[i].SuperRate.ShouldBe(expectedRecords[i].SuperRate);
                var actualPeriod = actualEmployees[i].PaymentStartDate;
                var expectedPeriod = expectedRecords[i].PaymentStartDate;
                actualPeriod.StartDate.ShouldBe(expectedPeriod.StartDate);
                actualPeriod.EndDate.ShouldBe(expectedPeriod.EndDate);
            }
        }

        [Category("Integration")]
        [Fact]
        public void ReadRecordsFromStream_ReadFileStream_DisposesStream()
        {
            // arrange & act
            using (var fileStream = CsvFile())
            {
                BuildCsvEmployeeRecordRepository().ReadRecordsFromStream<Employee>(fileStream);
                // assert
                Assert.Throws<ObjectDisposedException>(() => fileStream.ReadByte());
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