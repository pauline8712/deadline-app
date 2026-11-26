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

    [Fact]
    public void Newline_also_separates_numbers()
    {
        Assert.Equal(6, _calculator.Add("1\n2,3"));
    }

    [Fact]
    public void Custom_delimiter_can_be_set_on_the_first_line()
    {
        Assert.Equal(3, _calculator.Add("//;\n1;2"));
    }

    [Fact]
    public void Negative_numbers_throw_and_list_every_negative()
    {
        var error = Assert.Throws<ArgumentException>(() => _calculator.Add("1,-2,3,-4"));

        Assert.Equal("negatives not allowed: -2, -4", error.Message);
    }

    [Theory]
    [InlineData("2,1001", 2)]
    [InlineData("2,1000", 1002)]
    public void Numbers_bigger_than_1000_are_ignored(string input, int expected)
    {
        Assert.Equal(expected, _calculator.Add(input));
    }

    [Fact]
    public void Custom_delimiter_can_be_any_length()
    {
        Assert.Equal(6, _calculator.Add("//[***]\n1***2***3"));
    }

    [Theory]
    [InlineData("//[*][%]\n1*2%3", 6)]
    [InlineData("//[**][%%%]\n1**2%%%3", 6)]
    public void Several_custom_delimiters_can_be_set(string input, int expected)
    {
        Assert.Equal(expected, _calculator.Add(input));
    }
}
