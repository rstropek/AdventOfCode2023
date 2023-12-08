var input = File.ReadAllText("data.txt").Split("\n\n");

var directions = input[0];
var rules = input[1].Split('\n')
    .Select(l => new KeyValuePair<string, (string l, string r)>(l[0..3], (l[7..10], l[12..15])))
    .ToDictionary();

Part2(r => r == "AAA", r => r == "ZZZ");
Part2(r => r[^1] == 'A', r => r[^1] == 'Z');

void Part2(Func<string, bool> start, Func<string, bool> end)
{
    var steps = 0;
    var locations = rules.Keys.Where(start).ToArray();
    var zs = new long[locations.Length];
    var directionIx = 0;
    while (!zs.All(ix => ix != 0))
    {
        for (var i = 0; i < locations.Length; i++)
        {
            if (zs[i] != 0) { continue; }

            locations[i] = directions[directionIx] switch
            {
                'R' => rules[locations[i]].r,
                'L' => rules[locations[i]].l,
                _ => throw new Exception("Invalid direction")
            };

            if (end(locations[i])) { zs[i] = steps + 1; }
        }
        steps++;
        directionIx = (directionIx + 1) % directions.Length;
    }

    Console.WriteLine(FindLeastCommonMultiple(zs));
}

static long FindLeastCommonMultiple(long[] numbers)
{
    var result = numbers[0];
    for (int i = 1; i < numbers.Length; i++)
    {
        result = LeastCommonMultiple(result, numbers[i]);
    }

    return result;
}

// Calculates the LCM of two numbers based on the formula: LCM(a, b) = a * b / GCD(a, b)
// For details see e.g. https://www.idomaths.com/hcflcm.php#formula
static long LeastCommonMultiple(long a, long b) => a * b / GreatestCommonDivisor(a, b);

// Computes the Greatest Common Divisor (GCD) of two numbers using the Euclidean algorithm.
// For details see e.g. https://www.khanacademy.org/computing/computer-science/cryptography/modarithmetic/a/the-euclidean-algorithm
static long GreatestCommonDivisor(long a, long b)
{
    while (b != 0)
    {
        long temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}