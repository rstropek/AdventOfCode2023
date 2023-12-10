using System.Diagnostics;
using System.Numerics;

var input = File.ReadAllText("data.txt").Split('\n').Select(x => x.ToCharArray()).ToArray();

// Find x and y coordinates of "S"
var s = new Point(-1, -1);
for (int row = 0; row < input.Length; row++)
{
    for (int col = 0; col < input[row].Length; col++)
    {
        if (input[row][col] == 'S')
        {
            s = new(col, row);
            break;
        }
    }
}

// Find first pipe connection above, right, below, and left of "S"
Direction[] directions = [ new(0, -1), new(1, 0), new(0, 1), new(-1, 0) ];
var firstNonDot = new Point();
for (int i = 0; i < directions.Length && firstNonDot.X == -1; i++)
{
    var next = s + directions[i];
    if (!(next.X < 0 || next.X >= input[0].Length || next.Y < 0 || next.Y >= input.Length) 
        && ((directions[i].Dx == 1 && input[next.Y][next.X] is '-' or 'J' or '7')
            || (directions[i].Dx == -1 && input[next.Y][next.X] is '-' or 'F' or 'L')
            || (directions[i].Dy == 1 && input[next.Y][next.X] is '|' or 'L' or 'J')
            || (directions[i].Dy == -1 && input[next.Y][next.X] is '|' or 'F' or '7')))
    {
        firstNonDot = next;
        break;
    }
}

// Find all edges
var edges = new List<Edge>();
var path = new HashSet<Point>([s, firstNonDot]);
var edgeStart = s;
var current = firstNonDot;
var directionVector = new Direction(firstNonDot.X - s.X, firstNonDot.Y - s.Y);
do
{
    while (input.GetAt(current) is '-' or '|')
    {
        current += directionVector;
        path.Add(current);
    }
    edges.Add(new(edgeStart, current));
    edgeStart = current;

    directionVector = input.GetAt(current) switch
    {
        'L' or '7' => new(directionVector.Dy, directionVector.Dx),
        'J' or 'F' => new(-directionVector.Dy, -directionVector.Dx),
        'S' => directionVector,
        _ => throw new Exception("Invalid pipe"),
    };

    current += directionVector;
    path.Add(current);
} while (edgeStart != s);

Console.WriteLine(path.Count / 2);

// Make it a little bit faster by using a parallel loop
var insides = new bool[input.Length * input[0].Length - 1];
Parallel.For(0, insides.Length, i =>
{
    var row = i / input[0].Length;
    var col = i % input[0].Length;
    if (!path.Contains(new(col, row)) && PointInPoly(new(col, row), edges))
    {
        insides[i] = true;
    }
});

Console.WriteLine(insides.Count(x => x));

// Use the point-in-polygon algorithm (see also https://observablehq.com/@tmcw/understanding-point-in-polygon)
static bool PointInPoly(Point point, List<Edge> edges)
{
    var (x, y) = point;

    var inside = false;
    foreach(var edge in edges) {
        var xi = edge.Start.X;
        var yi = edge.Start.Y;
        var xj = edge.End.X;
        var yj = edge.End.Y;

        var intersect = ((yi > y) != (yj > y)) && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
        if (intersect) inside = !inside;
    }

    return inside;
}

record struct Point(int X, int Y) : IAdditionOperators<Point, Direction, Point>
{
    public Point() : this(-1, -1) { }
    public static Point operator +(Point p, Direction d) => new(p.X + d.Dx, p.Y + d.Dy);
}

record struct Edge(Point Start, Point End)
{
    public readonly int Length => Math.Abs(Start.X - End.X) + Math.Abs(Start.Y - End.Y);
}

record struct Direction(int Dx, int Dy);

static class ArrayExtension
{
    public static char GetAt(this char[][] content, Point point) => content[point.Y][point.X];
}
