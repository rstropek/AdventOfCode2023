using System.Diagnostics;

var lines = File.ReadAllLines("data.txt");

var sw = Stopwatch.StartNew();
var worthTotal = lines.Sum(CalculateWorth);
sw.Stop();
Console.WriteLine($"Worth: {worthTotal} in {sw.Elapsed.Microseconds}µs");

sw = Stopwatch.StartNew();
int total = lines.Length;
Span<int> instances = stackalloc int[lines.Length];
for (var i = 0; i < instances.Length; i++) { instances[i] = 1; }
for (var ix = 0; ix < lines.Length; ix++)
{
    var matches = Matches(lines[ix]);
    for (var j = 0; j < instances[ix]; j++)
    {
        for (var i = 1; i <= matches && ix + i < lines.Length; i++)
        {
            instances[ix + i]++;
            total++;
        }
    }
}
sw.Stop();

Console.WriteLine($"Total: {total} in {sw.Elapsed.Microseconds}µs");

static int CalculateWorth(string line)
{
    var matches = Matches(line);
    return matches > 0 ? (int)Math.Pow(2, matches - 1) : 0;
}

static int Matches(string line)
{
    var lineSpan = line.AsSpan();
    var left = lineSpan[10..];
    var right = lineSpan[42..];
    var matches = 0;
    for (var lix = 0; lix < 29; lix += 3)
    {
        for (var rix = 0; rix < 74; rix += 3)
        {
            if (left[lix] == right[rix] && left[lix + 1] == right[rix + 1])
            {
                matches++;
            }
        }
    }

    return matches;
}
