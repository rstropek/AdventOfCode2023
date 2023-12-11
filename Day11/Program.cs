var input = File.ReadAllLines("data.txt");

var galaxies = new List<Galaxy>();
for (var y = 0; y < input.Length; y++)
{
    for (var x = 0; x < input[y].Length; x++)
    {
        if (input[y][x] == '#')
        {
            galaxies.Add(new Galaxy(x, y));
        }
    }
}

Calculate(2);
Calculate(1000000);

void Calculate(int replaceBy)
{
    var newGalaxies = galaxies.ToList();
    for (var y = 0; y < input.Length; y++)
    {
        if (!galaxies.Any(g => g.Y == y))
        {
            for (var s = y + 1; s < input.Length; s++)
            {
                for (var g = 0; g < galaxies.Count; g++)
                {
                    if (galaxies[g].Y == s)
                    {
                        newGalaxies[g] = newGalaxies[g] with { Y = newGalaxies[g].Y + replaceBy - 1 };
                    }
                }
            }
        }
    }

    for (var x = 0; x < input[0].Length; x++)
    {
        if (!galaxies.Any(g => g.X == x))
        {
            for (var s = x + 1; s < input[0].Length; s++)
            {
                for (var g = 0; g < galaxies.Count; g++)
                {
                    if (galaxies[g].X == s)
                    {
                        newGalaxies[g] = newGalaxies[g] with { X = newGalaxies[g].X + replaceBy - 1 };
                    }
                }
            }
        }
    }

    var distance = newGalaxies
        .SelectMany((g, i) => newGalaxies.Skip(i + 1).Select(g2 => (g, g2)))
        .Select(p => p.g.DistanceFrom(p.g2))
        .Sum();
    Console.WriteLine(distance);
}

record struct Galaxy(long X, long Y)
{
    public readonly long DistanceFrom(Galaxy other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
}