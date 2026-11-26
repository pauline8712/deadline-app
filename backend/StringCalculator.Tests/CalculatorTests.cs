using StringCalculator;

namespace StringCalculator.Tests;

public sealed class CalculatorTests
{
    private readonly Calculator _calculator = new();

    [Fact]
    public void Empty_string_returns_0()
    {
        Assert.Equal(0, _calculator.Add(""));
    }

    [Fact]
    public void Single_number_returns_that_number()
    {
        Assert.Equal(1, _calculator.Add("1"));
    }

    [Fact]
    public void Two_numbers_separated_by_comma_returns_their_sum()
    {
        Assert.Equal(3, _calculator.Add("1,2"));
    }

    [Fact]
    public void Any_amount_of_numbers_returns_their_sum()
    {
        Assert.Equal(15, _calculator.Add("1,2,3,4,5"));
    }
}
