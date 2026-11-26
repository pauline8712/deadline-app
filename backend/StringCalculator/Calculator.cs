namespace StringCalculator;

public sealed class Calculator
{
    public int Add(string numbers)
    {
        if (numbers.Length == 0)
        {
            return 0;
        }

        var delimiters = new List<char> { ',', '\n' };
        if (numbers.StartsWith("//"))
        {
            delimiters.Add(numbers[2]);
            numbers = numbers[4..];
        }

        return numbers.Split(delimiters.ToArray()).Sum(int.Parse);
    }
}
