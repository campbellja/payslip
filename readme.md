# Payslip
Payslip is a web application for generating employee payslips. It accepts a single CSV input file containing employee information and outputs CSV file containig payslips for each employee's payment period.

# Prerequisites
1. .NET Core 2.1 SDK

## How to Build Payslip
1. Open a command prompt;
1. Enter the following commmand:

        ./build.cmd
        
        or

        dotnet restore
        dotnet build

## How to Run the Tests

        dotnet test .\Payslip.Tests

## How to Run Payslip
1. Execute the following to launch the web app using local hosting: 

        ./run.cmd

        or

        dotnet run --project .\Payslip.Web

2. Example Employee files are located here: 

        Payslip.Tests\TestData

## Packages used:
- AspNetCore `https://www.asp.net/`
    
    The application framework for this application's web frontend - part of .NET Core runtime.
- CsvHelper `https://joshclose.github.io/CsvHelper/`
    
    A CSV text parsing package for .net core - useful for reading/writing CSV file formats.

For testing:
- Xunit: `https://github.com/xunit/xunit`

    Unit-testing framework supported by .net core scaffolding utilities.
- Shouldly `https://github.com/shouldly/shouldly`

    An assertion framework for simple, concise test assertions.

# Design
Payslip is a ASP .NET Core MVC web application containing five assemblies:

`Payslip.Web`: Web frontend for interacting with the Payslip Service to generate Payslips;

`Payslip.Model`: Payslip calculation domain models and classes;

`Payslip.Service`: Contains the Payslip service for processing and outputting Payslips. This can be consumed by another service, console app or web app;

`Payslip.DataAccess`: For processing input files and outputting CSV files, and

`Payslip.Tests`: Unit tests and integration tests for testing file processing and calculation behaviours.

The payslip calculation logic operates on a `TaxRates` table defined in code (See: `Payslip.Model.Constants.TaxRates`).

## Employee Input Assumptions
`Annual Salary`: This is a positive integer and any negative or fractional values will not be accepted.

`Super Rate`: Superannuation rate percentages are appended with a `%` symbol in the Employee input file.

### Pay Periods
`Whole Months`: A Pay Period is defined as a date range of whole months; ranges for partial months are not valid input.
If the inputted date range exceeds one calender month, an additional payslip entry will be generated for the remainder.

`Implicit Year`: Unless specified, the year of the inputted Pay Periods are implicitly the current year on the web server hosting Payslip.

## Payslip Output Assumptions

### Ordered by Employee Input
The order of each employee in the input file determines the order of employee payslips outputted by the app.


