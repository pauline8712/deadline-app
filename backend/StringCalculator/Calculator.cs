namespace StringCalculator;

public sealed class Calculator
{
    public int Add(string numbers) =>
        numbers.Length == 0 ? 0 : numbers.Split(',').Sum(int.Parse);
}
