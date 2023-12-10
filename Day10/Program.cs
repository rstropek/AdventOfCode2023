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

var inside = 0;
for (var row = 0; row < input.Length; row++)
{
    for (var col = 0; col < input[row].Length; col++)
    {
        if (!path.Contains(new(col, row)) && PointInPoly(new(col, row), edges))
        {
            inside++;
        }
    }
}

System.Console.WriteLine(inside);

// Use the point-in-polygon algorithm (see also https://observablehq.com/@tmcw/understanding-point-in-polygon)
static bool PointInPoly(Point point, List<Edge> edges)
{
    var (x, y) = point;

    var inside = false;
    for (var i = 0; i < edges.Count; i++) {
        var xi = edges[i].Start.X;
        var yi = edges[i].Start.Y;
        var xj = edges[i].End.X;
        var yj = edges[i].End.Y;

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
