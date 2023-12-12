using System.Text;
using System.Text.RegularExpressions;

var input = """
???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1
""".Split('\n').Select(l =>
{
    var parts = l.Split(' ');
    var damages = parts[1].Split(',').Select(int.Parse).ToArray();
    return (
        string.Join('?', Enumerable.Range(0, 5).Select(_ => parts[0])), 
        damages.Concat(damages).Concat(damages).Concat(damages).Concat(damages).ToArray()
    );
})
.ToArray();

var sum = 0;
foreach(var item in input)
{
    var regexStr = GetRegex(item.Item2);
    var regex = new Regex(regexStr, RegexOptions.Compiled);
    var p = NumberOfPossibilities(item.Item1, regex);
    sum += p;
}

System.Console.WriteLine(sum);

static int NumberOfPossibilities(string pattern, Regex regex)
{
    var hash = pattern.IndexOf('?');
    if (hash == -1)
    {
        if (regex.IsMatch(pattern))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    return NumberOfPossibilities(pattern[..hash] + '#' + pattern[(hash + 1)..], regex) +
           NumberOfPossibilities(pattern[..hash] + '.' + pattern[(hash + 1)..], regex);
}

string GetRegex(int[] damaged)
{
    var sb = new StringBuilder(@"^\.*");

    for (var i = 0; i < damaged.Length; i++)
    {
        sb.Append("#{");
        sb.Append(damaged[i]);
        sb.Append('}');
        if (i < damaged.Length - 1)
        {
            sb.Append(@"\.+");
        }
    }

    sb.Append(@"\.*$");

    return sb.ToString();
}