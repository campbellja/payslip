<Query Kind="Program">
  <NuGetReference>NodaTime</NuGetReference>
  <Namespace>NodaTime</Namespace>
  <Namespace>NodaTime.Utility</Namespace>
</Query>

void Main()
{
	var today = DateTime.Today;	
	
	today.WeeksBetween(today.AddMonths(7)).Dump();
	today.WeeksBetween(today.AddDays(365)).Dump();
	today.MonthsBetween(today.AddDays(90)).Dump();
	today.MonthsBetween(today.AddDays(90)).Dump();
}

public static class DateTime_Extensions
{
	public static int WeeksBetween(this DateTime from, DateTime to )
	{
		var period = Period.Between(
			LocalDateTime.FromDateTime(from),
			LocalDateTime.FromDateTime(to)
		, PeriodUnits.Weeks);
		return period.Weeks;
	}

	public static int MonthsBetween(this DateTime from, DateTime to)
	{
		var period = Period.Between(
			LocalDateTime.FromDateTime(from),
			LocalDateTime.FromDateTime(to)
		, PeriodUnits.Months);
		return period.Months;
	}
}

// Define other methods and classes here
