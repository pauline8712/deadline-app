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
}
