namespace StringCalculator;

public sealed class Calculator
{
    public int Add(string numbers) => numbers == "" ? 0 : int.Parse(numbers);
}
