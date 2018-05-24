<Query Kind="Program" />


interface ICalc
{
	decimal AmountByPaymentFrequency(decimal amount);
	int PaymentPeriodsInDateRange(DateTime from, DateTime to);
}

class MonthlyPaymentCalc : ICalc
{
	public decimal AmountByPaymentFrequency(decimal amount)
	{		
		return amount / 12;		
	}

	public int PaymentPeriodsInDateRange(DateTime from, DateTime to)
	{
		//HACK for now
		return (int)((from - to).Days / 12);
	}
}

class FortnightlyPaymentCalc : ICalc
{
	public decimal AmountByPaymentFrequency(decimal amount)
	{
		return amount / 26;		
	}
	public int PaymentPeriodsInDateRange(DateTime from, DateTime to){
		return (int) ((from - to ).TotalDays / 26);
	}	
}



static class Decimal_Extensions
{
	public static decimal RoundUp(this decimal @decimal){
		return Math.Round(@decimal, 0, MidpointRounding.AwayFromZero);
	}
	public static decimal RoundDown(this decimal @decimal)
	{
		return Math.Floor(@decimal);
	}
}

public class Input
{
	public decimal AnnualSalary{get;set;}
	public decimal SuperAnnuationRatePercentage{get;set;}
}

void Main()
{
	new DateTime(2018,3,1).Subtract(new DateTime(2018,6,30))
	.Days
	.Dump();
	
	(from input in new[]{
		new Input{AnnualSalary = 5_400M,   	SuperAnnuationRatePercentage = 0.09M},
		new Input{AnnualSalary = 24_500M,	SuperAnnuationRatePercentage = 0.09M},
		new Input{AnnualSalary = 60_050M,   SuperAnnuationRatePercentage = 0.09M},
		new Input{AnnualSalary = 120_000M,  SuperAnnuationRatePercentage = 0.10M}
	}
	 from calc in new ICalc[]{
		 new MonthlyPaymentCalc(),		 
	}	
	let salary = input.AnnualSalary
	let superRate = input.SuperAnnuationRatePercentage
	let rate = TaxRates.Single(t=>t.IsWithinIncomeRange(salary))
	
	let incomeTaxForFrequency = calc.AmountByPaymentFrequency(rate.CalculateIncomeTax(salary))
		.RoundUp()
	let grossIncomeForFrequency = calc.AmountByPaymentFrequency(salary).RoundDown()
	let netIncome = grossIncomeForFrequency - incomeTaxForFrequency
	let super = CalculateSuperForGrossIncome(grossIncomeForFrequency, superRate).RoundDown()
	select new
	{
		For = (calc is MonthlyPaymentCalc)?"Monthly":"Fortnightly",		
		Salary = salary,
		TaxRate = rate.CalculateTaxAmountOverMinIncome(salary),
		GrossIncome = grossIncomeForFrequency,
		IncomeTax = incomeTaxForFrequency,		
		NetIncome = netIncome,		
		Super = super		
	})
	.Dump();

	// https://stackoverflow.com/questions/14/difference-between-math-floor-and-math-truncate/580252#580252
	var raw = 60_050M / 12M;
	Math.Round(raw, 0, MidpointRounding.AwayFromZero).Dump("Math.Round(raw,0,MidpointRounding.AwayFromZero");
	Math.Round(raw, 0, MidpointRounding.ToEven).Dump("Math.Round(raw,0,MidpointRounding.ToEven");
	Math.Ceiling(raw).Dump("Math.Ceiling(raw)");
	Math.Floor(raw).Dump("Math.Floor(raw)");

}

public class CalculationContext
{
	public decimal Salary{get;set;}
	public PaymentPeriod PaymentPeriod {get;set;}
	
	public CalculationContext(decimal salary, PaymentPeriod paymentPeriod)
	{
		Salary = salary;
		PaymentPeriod = paymentPeriod;
	}
}

decimal CalculateSuperForGrossIncome(decimal grossIncome, decimal ratePercentage)
{
	return grossIncome * ratePercentage;
}

public enum PaymentPeriod
{
	Unknown = 0,
	Fortnightly,
	Monthly
}



// Supplied 2018 Tax Table:
// $0 - $18,200         Nil Nil
// $18,201 - $37,000    19c for each $1 over $18,200
// $37,001 - $87,000    $3,572 plus 32.5c for each $1 over $37,000
// $87,001 - $180,000   $19,822 plus 37c for each $1 over $87,000
// $180,001 and over    $54,232 plus 45c for each $1 over $180,000 
TaxRate[] TaxRates => new[]{
	TaxRate.NilTaxRate( 0M, 18_200M),
	TaxRate.Create( 18_201M, 37_000M, 0.19M),
	TaxRate.Create( 37_001M, 87_000M, 0.325M,  3_572M ),
	TaxRate.Create( 87_001M, 180_000M, 0.37M, 19_822M),
	TaxRate.TopTierRate( 180_001M, 0.45M, 54_232M ),
};
       
public sealed class TaxRate
{
	public decimal MinIncome { get; set; }
	public decimal? MaxIncome { get; set; }
	public decimal? BaseTaxAmount { get; set; }
	public decimal? RateValue { get; set; }

	public TaxRate(decimal minIncome, decimal maxIncome) : this(minIncome, maxIncome, null, null) { }

	public static TaxRate NilTaxRate(decimal minIncome, decimal maxIncome)
	{
		return new TaxRate(minIncome, maxIncome);
	}

	internal static TaxRate Create(decimal minIncome, decimal? maxIncome = null, decimal? rateValue = null, decimal? baseTaxAmount = null)
	{
		return new TaxRate(minIncome, maxIncome, rateValue, baseTaxAmount);
	}

	internal static TaxRate TopTierRate(decimal minIncome, decimal? rateValue = null, decimal? baseTaxAmount = null)
	{
		return new TaxRate(minIncome, null, rateValue, baseTaxAmount);
	}

	public TaxRate(decimal minIncome, decimal? maxIncome = null, decimal? rateValue = null, decimal? baseTaxAmount = null)
	{
		if (minIncome < 0M)
		{
			throw new ArgumentOutOfRangeException(nameof(minIncome), $"{nameof(minIncome)} must not be a negative decimal");
		}
		if (maxIncome.HasValue && maxIncome.Value < 0M)
		{
			throw new ArgumentOutOfRangeException(nameof(maxIncome), $"{nameof(maxIncome)} must not be a negative decimal");
		}
		if (maxIncome < minIncome)
		{
			throw new ArgumentOutOfRangeException(nameof(maxIncome), $"{nameof(maxIncome)} must be greater than {nameof(minIncome)}");
		}
		if (baseTaxAmount.HasValue && baseTaxAmount < 0M)
		{
			throw new ArgumentOutOfRangeException(nameof(baseTaxAmount), $"{nameof(baseTaxAmount)} must not be a negative decimal");
		}
		MinIncome = minIncome;
		MaxIncome = maxIncome;
		BaseTaxAmount = baseTaxAmount;
		RateValue = rateValue;
	}

	public bool IsWithinIncomeRange(decimal salary) => salary >= MinIncome && (!MaxIncome.HasValue || (salary <= MaxIncome));

	// taxAmountOverMinIncome = (Income - MinIncomeThresholdAmount) x rateForEachDollarOverMinIncome;
	public decimal CalculateTaxAmountOverMinIncome(decimal salary)
	{
		if (!RateValue.HasValue)
		{
			return 0.0M;
		}
		decimal minIncomeThresholdAmount;
		if (MinIncome > 0)
		{
			minIncomeThresholdAmount = MinIncome - 1M;
		}
		else
		{
			minIncomeThresholdAmount = MinIncome;
		}
		var rate = RateValue.Value;
		var taxAmountOverMinIncome = (salary - minIncomeThresholdAmount) * rate;
		return taxAmountOverMinIncome;
	}

	//incomeTax = (BaseTaxAmount + taxAmountOverMinIncome) / monthsInOneYear = incomeTax (rounded up)
	public decimal CalculateIncomeTax(decimal annualSalary)
	{	
		if (!BaseTaxAmount.HasValue && !RateValue.HasValue)
		{
			return 0.0M;
		}
		
		var taxAmountOverMinIncome = CalculateTaxAmountOverMinIncome(annualSalary);
		decimal baseTax;
		if (BaseTaxAmount.HasValue)
		{
			baseTax = BaseTaxAmount.Value;
		}
		else
		{
			baseTax = 0.0M;
		}
		return baseTax + taxAmountOverMinIncome;
	}

}