using StringCalculator;

namespace StringCalculator.Tests;

public sealed class CalculatorTests
{
    private readonly Calculator _calculator = new();

    [Theory]
    [InlineData("", 0)]
    [InlineData("1", 1)]
    [InlineData("1,2", 3)]
    [InlineData("1,2,3,4,5", 15)]
    public void Comma_separated_numbers_are_summed(string input, int expected)
    {
        Assert.Equal(expected, _calculator.Add(input));
    }
}
