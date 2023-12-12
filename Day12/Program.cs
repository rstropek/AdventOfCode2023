using System.Text.RegularExpressions;

//ConcurrentDictionary<string, int> SectionCountCache = new();
var input = """
?.#?????????###.?# 1,1,2,1,5,1
""".Split('\n').Select(l =>
{
    var parts = l.Split(' ');
    var damages = parts[1].Split(',').Select(int.Parse).ToArray();
    return (
        pattern: ProcessString(parts[0]),
        //ProcessString(string.Join('?', Enumerable.Range(0, 5).Select(_ => parts[0]))), 
        groupLengths: damages
        //damages.Concat(damages).Concat(damages).Concat(damages).Concat(damages).ToArray()
    );
})
.ToArray();

var line = input[0];

// Find first # from end
var firstHashFromEnd = line.pattern.LastIndexOf('#');
var charsToRight = line.pattern.Length - 1 - firstHashFromEnd;
var numberOfHashesToLeft = Math.Max(0, line.groupLengths[^1] - charsToRight - 1);
var newPattern = line.pattern[..(firstHashFromEnd - numberOfHashesToLeft)];
newPattern += new string('#', numberOfHashesToLeft);
newPattern += line.pattern[firstHashFromEnd..];
System.Console.WriteLine(line.pattern);
System.Console.WriteLine(newPattern);
line.pattern = newPattern;

if (line.pattern.EndsWith('#'))
{
    newPattern = line.pattern[..^(line.groupLengths[^1] + 1)];
    newPattern += ".";
    newPattern += new string('#', line.groupLengths[^1]);
}

System.Console.WriteLine(line.pattern);
System.Console.WriteLine(newPattern);

static string ProcessString(string input)
{
    var trimmed = input.Trim('.');
    var result = DotDuplicationRemovalRegex().Replace(trimmed, ".");
    return result;
}

partial class Program
{
    [GeneratedRegex(@"\.{2,}")]
    private static partial Regex DotDuplicationRemovalRegex();
}