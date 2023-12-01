Linq();
await Simple();

static void Linq()
{
    // Part one can be expressed in LINQ pretty easily
    Console.WriteLine(File.ReadAllLines("data.txt")
        .Select(line => (
            line[line.IndexOfAny(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'])] - '0',
            line[line.LastIndexOfAny(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'])] - '0'))
        .Sum(x => x.Item1 * 10 + x.Item2));
}

static async Task Simple()
{
    var sum = 0;
    var spelledOut = new (string text, int value)[] {
        ("one", 1),
        ("two", 2),
        ("three", 3),
        ("four", 4),
        ("five", 5),
        ("six", 6),
        ("seven", 7),
        ("eight", 8),
        ("nine", 9),
    };
    await foreach (var line in File.ReadLinesAsync("data.txt"))
    {
        int first = 0, last = 0;
        var found = false;
        for (var i = 0; !found && i < line.Length; i++)
        {
            if (char.IsDigit(line[i])) { first = line[i] - '0'; found = true; }

            // Part 2
            for (var j = 0; !found && j < spelledOut.Length; j++)
            {
                if (line[i..].StartsWith(spelledOut[j].text))
                {
                    first = spelledOut[j].value;
                    found = true;
                }
            }
        }

        found = false;
        for (var i = line.Length - 1; !found && i >= 0; i--)
        {
            if (char.IsDigit(line[i])) { last = line[i] - '0'; found = true; }

            // Part 2
            for (var j = 0; j < spelledOut.Length; j++)
            {
                if (line[..(i + 1)].EndsWith(spelledOut[j].text))
                {
                    last = spelledOut[j].value;
                    found = true;
                }
            }
        }

        sum += first * 10 + last;
    }

    Console.WriteLine(sum);
}