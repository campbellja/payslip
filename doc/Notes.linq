<Query Kind="Program" />

void Main()
{
	(from c in new ICalc[]{new MonthlyPaymentCalc(),new FortnightlyPaymentCalc()}
	let amount = c.AmountByPaymentFrequency(60_500M)
	select new{
		GrossValue = amount,
		GrossValue_Rounded = Math.Round(amount,0,MidpointRounding.AwayFromZero)
		}
	)
	.Dump();
}

interface ICalc 
{
	decimal AmountByPaymentFrequency(decimal amount);
}

class MonthlyPaymentCalc : ICalc
{
	public decimal AmountByPaymentFrequency(decimal amount)
	{
		return amount / 12;
	}
}

class FortnightlyPaymentCalc : ICalc
{
	public decimal AmountByPaymentFrequency(decimal amount)
	{
		return amount / 26;
	}
}
// Define other methods and classes here