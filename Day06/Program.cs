var lines = """
Time:        49     97     94     94
Distance:   263   1532   1378   1851
""".Split('\n').Select(line => line[9..]);

var input = lines
    .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
    .Select(line => line.Select(int.Parse).ToArray())
    .ToArray();
var res = Calculate(input[0].Select((t, ix) => new Race(t, input[1][ix])));
Console.WriteLine(res);

var input2 = lines
    .Select(line => line.Replace(" ", ""))
    .Select(long.Parse)
    .ToArray();
res = Calculate([new Race(input2[0], input2[1])]);
Console.WriteLine(res);

static long Calculate(IEnumerable<Race> races)
{
    long result = 1;
    foreach (var race in races)
    {
        var x = (long)Math.Ceiling((race.Time - Math.Sqrt(race.Time * race.Time - 4 * race.Distance)) / 2);
        if ((race.Time - x) * x == race.Distance) { x++; }
        long wins = race.Time - 2 * x + 1;
        result *= wins;
    }

    return result;
}

record struct Race(long Time, long Distance);
