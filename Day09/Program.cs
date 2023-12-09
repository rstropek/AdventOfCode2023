var lines = File.ReadAllText("data.txt").Split('\n').Select(l => l.Split(' ').Select(int.Parse).ToList()).ToArray();

Part1(Kind.Back);
Part1(Kind.Front);

void Part1(Kind kind)
{
    var extrapolated = new List<int>();
    foreach (var line in lines)
    {
        var pyramidLevels = new List<List<int>> { line };
        List<int> nextLevel;
        do
        {
            var level = pyramidLevels[^1];
            nextLevel = [];
            for (var i = 0; i < level.Count - 1; i++)
            {
                nextLevel.Add(level[i + 1] - level[i]);
            }

            pyramidLevels.Add(nextLevel);
        }
        while (!nextLevel.All(d => d == 0));

        if (kind == Kind.Back) { pyramidLevels[^1].Add(0); }
        else { pyramidLevels[^1].Insert(0, 0); }
        for (var i = pyramidLevels.Count - 2; i >= 0; i--)
        {
            int ex;
            if (kind == Kind.Back)
            { 
                ex = pyramidLevels[i][^1] + pyramidLevels[i + 1][^1]; 
                pyramidLevels[i].Add(ex);
            }
            else
            { 
                ex = pyramidLevels[i][0] - pyramidLevels[i + 1][0]; 
                pyramidLevels[i].Insert(0, ex);
            }
        }

        if (kind == Kind.Back) { extrapolated.Add(pyramidLevels[0][^1]); }
        else { extrapolated.Add(pyramidLevels[0][0]); }
    }

    Console.WriteLine(extrapolated.Sum());
}

enum Kind { Front, Back };
