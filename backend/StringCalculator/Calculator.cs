namespace StringCalculator;

public sealed class Calculator
{
    public int Add(string numbers)
    {
        if (numbers.Length == 0)
        {
            return 0;
        }

        var delimiters = new List<string> { ",", "\n" };
        if (numbers.StartsWith("//["))
        {
            var end = numbers.IndexOf("]\n", StringComparison.Ordinal);
            delimiters.Add(numbers[3..end]);
            numbers = numbers[(end + 2)..];
        }
        else if (numbers.StartsWith("//"))
        {
            delimiters.Add(numbers[2].ToString());
            numbers = numbers[4..];
        }

        var values = numbers.Split(delimiters.ToArray(), StringSplitOptions.None).Select(int.Parse).ToList();
        var negatives = values.Where(v => v < 0).ToList();
        if (negatives.Count > 0)
        {
            throw new ArgumentException($"negatives not allowed: {string.Join(", ", negatives)}");
        }

        return values.Where(v => v <= 1000).Sum();
    }
}
