var input = File.ReadAllText("data.txt");

// Process first line
var endOfLine1 = input.IndexOf('\n');
var seeds = input[7..endOfLine1].Split(' ').Select(long.Parse).ToArray();
input = input[(endOfLine1 + 2)..];

// Part 1 and part 2 are calculated the same way. In part 1, the seed ranges have length 1.
var seedRanges = seeds.Select(s => new SeedRange(s, 1)).ToList();
Calculate(input, seedRanges);

seedRanges = Enumerable.Range(0, seeds.Length / 2)
    .Select(i => new SeedRange(seeds[i * 2], seeds[i * 2 + 1])).ToList();
Calculate(input, seedRanges);

static void Calculate(string input, List<SeedRange> seedRanges)
{
    // Split transformations into sections
    var sections = input.Split("\n\n")
        .Select(section => section.Split('\n').Skip(1)
            .Select(l => l.Split(' ').Select(long.Parse).ToArray())
            .Select(l => new Map(l[0], l[1], l[2])))
        .ToArray();

    foreach(var section in sections)
    {
        // Helper variable receiving the next seed ranges
        var nextSeedRanges = new List<SeedRange>();
        foreach (var map in section)
        {
            while (true)
            {
                // Find the first seed range that overlaps with the current map
                var osr = seedRanges.FirstOrDefault(s => s.Start < map.SourceRangeEnd && s.End > map.SourceRangeStart);
                if (osr.Length == 0) { break; }

                // Remove the original seed range because it will have been processed
                seedRanges.Remove(osr);

                // Calculate the overlapping range
                var overlapStart = Math.Max(map.SourceRangeStart, osr.Start);
                var overlapEnd = Math.Min(map.SourceRangeEnd, osr.End);

                // Add the remaining parts of the original seed range, they will maybe overlap with other maps
                if (osr.Start < overlapStart) { seedRanges.Add(SeedRange.FromStartEnd(osr.Start, overlapStart - 1)); }
                if (osr.End > overlapEnd) { seedRanges.Add(SeedRange.FromStartEnd(overlapEnd + 1, osr.End)); }

                // Add the transformed range to the next seed ranges
                nextSeedRanges.Add(SeedRange.FromStartEnd(overlapStart + map.Delta, overlapEnd + map.Delta));
            }
        }

        nextSeedRanges.AddRange(seedRanges);

        seedRanges = nextSeedRanges;
    }

    Console.WriteLine(seedRanges.Min(sr => sr.Start));
}

record struct SeedRange(long Start, long Length)
{
    public static SeedRange FromStartEnd(long start, long end) => new(start, end - start + 1);
    public readonly long End => Start + Length - 1;
}

record struct Map(long DestRangeStart, long SourceRangeStart, long RangeLength)
{
    public readonly long DestRangeEnd => DestRangeStart + RangeLength - 1;
    public readonly long SourceRangeEnd => SourceRangeStart + RangeLength - 1;
    public readonly long Delta => DestRangeStart - SourceRangeStart;
}
