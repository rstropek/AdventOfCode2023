var data = File.ReadLines("data.txt")
    .Select(line => line.Split(": "))
    .Select(parts => new {
        GameId = int.Parse(parts[0].Split(' ')[1]),
        Sections = parts[1].Split("; ")
            .Select(section => section.Split(", ")
                .Select(colors => colors.Split(' '))
                .Select(colors => new {
                    Amount = int.Parse(colors[0]),
                    Color = colors[1]
                }))
            .Select(section => new {
                Red = section.Where(cubes => cubes.Color == "red").FirstOrDefault()?.Amount ?? 0,
                Green = section.Where(cubes => cubes.Color == "green").FirstOrDefault()?.Amount ?? 0,
                Blue = section.Where(cubes => cubes.Color == "blue").FirstOrDefault()?.Amount ?? 0,
            })
    });

Console.WriteLine(data
    .Where(game => game.Sections.All(s => s.Red <= 12 && s.Green <= 13 && s.Blue <= 14))
    .Sum(game => game.GameId));

Console.WriteLine(data
    .Select(game => new {
        MaxRed = game.Sections.Max(s => s.Red),
        MaxGreen = game.Sections.Max(s => s.Green),
        MaxBlue = game.Sections.Max(s => s.Blue),
    })
    .Select(game => game.MaxRed * game.MaxGreen * game.MaxBlue)
    .Sum());
