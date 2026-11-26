namespace StringCalculator;

public sealed class Calculator
{
    public int Add(string numbers)
    {
        if (numbers == "")
        {
            return 0;
        }

        var sum = 0;
        foreach (var part in numbers.Split(','))
        {
            sum += int.Parse(part);
        }

        return sum;
    }
}
