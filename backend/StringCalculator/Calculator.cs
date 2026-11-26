namespace StringCalculator;

/// <summary>Roy Osheroves String Calculator-kata, byggd ett krav i taget med TDD.</summary>
public sealed class Calculator
{
    private const int MaxValue = 1000;
    private static readonly string[] DefaultDelimiters = [",", "\n"];

    public int Add(string numbers)
    {
        if (numbers.Length == 0)
        {
            return 0;
        }

        var (delimiters, body) = ParseHeader(numbers);
        var values = body.Split(delimiters, StringSplitOptions.None).Select(int.Parse).ToList();
        ThrowIfAnyNegative(values);
        return values.Where(v => v <= MaxValue).Sum();
    }

    /// <summary>Läser en valfri första rad: "//;\n", "//[***]\n" eller "//[*][%]\n".</summary>
    private static (string[] Delimiters, string Body) ParseHeader(string numbers)
    {
        if (!numbers.StartsWith("//", StringComparison.Ordinal))
        {
            return (DefaultDelimiters, numbers);
        }

        var headerEnd = numbers.IndexOf('\n');
        var header = numbers[2..headerEnd];
        var custom = header.StartsWith('[')
            ? header[1..^1].Split("][", StringSplitOptions.None)
            : [header];
        return ([.. DefaultDelimiters, .. custom], numbers[(headerEnd + 1)..]);
    }

    private static void ThrowIfAnyNegative(IEnumerable<int> values)
    {
        var negatives = values.Where(v => v < 0).ToList();
        if (negatives.Count > 0)
        {
            throw new ArgumentException($"negatives not allowed: {string.Join(", ", negatives)}");
        }
    }
}
