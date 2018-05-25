# Payslip
Payslip is a web application for generating employee payslips. It accepts an input file containing employee information and outputs payments for each employee's payment period.


# Prerequisites
1. .NET Core 2.1 SDK

# Using Payslip
Payslip is an asp.net core application 

## How to Build & Test
1. Open a command prompt;
1. Run `build.cmd`, or alternatively enter:

        dotnet restore
        dotnet build
        # Also, run all of the unit tests
        dotnet test        

## How to Run Payslip
1. Execute the following to launch the web app using local hosting: 

    `dotnet run --project .\Payslip.Web`

## Packages used:
- AspNetCore `https://www.asp.net/`
    
    The application framework for this application's web frontend.
- CsvHelper `https://joshclose.github.io/CsvHelper/`
    
    A popular CSV Parser package for .net core - useful for reading/writing CSV file formats.

# Design
## Assumptions

### Pay Periods
A Pay Period is defined as a date range of whole months; ranges for partial months are not valid input.
If the inputted date range exceeds one calender month, an additional payslip entry will be generated for the remainder.

### Output Format

### Order
The output payslips are ordered chronologically by Pay Period.


