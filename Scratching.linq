<Query Kind="Program" />

void Main()
{
	(from salary in new[]{
		5_400M,
		24_500M,
		60_500M,
		120_000M
	}
	let rate = TaxRates.Single(t=>t.IsWithinIncomeRange(salary))
	let monthlyIncomeTax = rate.CalculateIncomeTax(salary)
	select new{
		Salary = salary,
		TaxRate = rate.CalculateTaxAmountOverMinIncome(salary),
		Monthly_IncomeTax = monthlyIncomeTax,
		Yearly_IncomeTax = monthlyIncomeTax * 12
	})
	.Dump();
}

decimal CalculateTax(decimal x)
{
	throw new NotImplementedException();
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
	public decimal CalculateIncomeTax(decimal salary)
	{
		if (!BaseTaxAmount.HasValue && !RateValue.HasValue)
		{
			return 0.0M;
		}
		const int monthsInOneYear = 12;
		var taxAmountOverMinIncome = CalculateTaxAmountOverMinIncome(salary);
		decimal baseTax;
		if (BaseTaxAmount.HasValue)
		{
			baseTax = BaseTaxAmount.Value;
		}
		else
		{
			baseTax = 0.0M;
		}
		var incomeTax = (baseTax + taxAmountOverMinIncome) / monthsInOneYear;
		var incomeTaxRounded = Math.Round(incomeTax, 0);
		return incomeTaxRounded;
	}
}