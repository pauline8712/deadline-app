namespace StringCalculator;

public sealed class Calculator
{
    public int Add(string numbers)
    {
        if (numbers == "")
        {
            return 0;
        }

        var parts = numbers.Split(',');
        return parts.Length == 1 ? int.Parse(parts[0]) : int.Parse(parts[0]) + int.Parse(parts[1]);
    }
}
