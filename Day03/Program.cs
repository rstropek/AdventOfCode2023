using System.Text.RegularExpressions;

var lines = File.ReadAllLines("data.txt");

var specialCharsCoords = lines
    .SelectMany((line, y) => line
        .Select((c, x) => (c, x))
        .Where(t => "*&/@=+#$%-".Contains(t.c))
        .Select(t => new SpecialChar(t.c, t.x, y)))
    .ToArray();

var numbers = lines
    .SelectMany((line, y) => NumberRect.NumberRegex().Matches(line)
        .Select(m => new NumberRect(int.Parse(m.Value), m.Index - 1, y - 1, m.Index + m.Length, y + 1)))
    .ToArray();
Console.WriteLine(numbers.Where(n => specialCharsCoords.Any(s => s.IsInRect(n))).Sum(n => n.Value));

Console.WriteLine(specialCharsCoords.Where(c => c.C == '*')
    .Select(c => numbers.Where(n => c.IsInRect(n)))
    .Where(c => c.Count() == 2)
    .Sum(c => c.Aggregate(1, (acc, n) => acc * n.Value)));

partial record struct NumberRect(int Value, int X1, int Y1, int X2, int Y2)
{
    [GeneratedRegex(@"\d+")]
    public static partial Regex NumberRegex();
}

record struct SpecialChar(char C, int X, int Y)
{
    public readonly bool IsInRect(NumberRect r) 
        => X >= r.X1 && X <= r.X2 && Y >= r.Y1 && Y <= r.Y2;
};
