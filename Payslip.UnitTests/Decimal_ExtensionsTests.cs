using Payslip.Model;
using Shouldly;
using Xunit;

namespace Payslip.UnitTests
{
    public sealed class Decimal_ExtensionsTests
    {
        [Fact]
        public void RoundUp_DecimalGreaterThanEqualTo50_RoundToNextDollar()
        {
            0.50M.RoundToNearestDollar().ShouldBe(1M);
            0.50M.RoundToNearestDollar().ShouldBe(1M);
            0.449M.RoundToNearestDollar().ShouldBe(0M);
            0.501M.RoundToNearestDollar().ShouldBe(1M);
            0.49999M.RoundToNearestDollar().ShouldBe(0M);
        }

        [Fact]
        public void IsWholeNumber_FractionNumber_ReturnsFalse()
        {
            0.01M.IsWholeNumber().ShouldBeFalse();
            0.001M.IsWholeNumber().ShouldBeFalse();
            1.1M.IsWholeNumber().ShouldBeFalse();
        }
    }
}