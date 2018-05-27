# Payslip
Payslip is a web application for generating employee payslips. It accepts an input file containing employee information and outputs payments for each employee's payment period.


# Prerequisites
1. .NET Core 2.1 SDK

## How to Build Payslip
1. Open a command prompt;
1. Run `build.cmd`, or alternatively enter:

        dotnet restore
        dotnet build

## How to Run the Tests

        dotnet test .\Payslip.UnitTests

## How to Run Payslip
1. Execute the following to launch the web app using local hosting: 

        dotnet run --project .\Payslip.Web

2. An example Employee file is located here: 

        Payslip.UnitTests\TestData\input.csv

## Packages used:
- AspNetCore `https://www.asp.net/`
    
    The application framework for this application's web frontend - part of .NET Core runtime.
- CsvHelper `https://joshclose.github.io/CsvHelper/`
    
    A CSV text parsing package for .net core - useful for reading/writing CSV file formats.

# Design
## Assumptions

### Super Rate
Superannuation rate percentages are appended with a `%` symbol in the Employee input file.

### Pay Periods
`Whole Months`: A Pay Period is defined as a date range of whole months; ranges for partial months are not valid input.
If the inputted date range exceeds one calender month, an additional payslip entry will be generated for the remainder.

`Implicit Year`: Unless specified, the year of the inputted Pay Periods are implicitly the current year on the web server hosting Payslip.

### Output Format

### Order
The output payslips are ordered chronologically by Pay Period.


