using System.Diagnostics.CodeAnalysis;

const char OPEN = '.';
const char TREE = '|';
const char LUMBERYARD = '#';

var input = File.ReadAllLines("input.txt");

var answer1 = Simulate(input, 10);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = Simulate(input, 1_000_000_000);
Console.WriteLine($"Answer 2: {answer2}");

char[,] Parse(string[] input)
{
    var result = new char[input.Length, input[0].Length];

    for (int row = 0; row < result.GetLength(0); row++)
    {
        for (int column = 0; column < result.GetLength(1); column++)
        {
            result[row, column] = input[row][column];
        }
    }

    return result;
}

bool IsValid(char[,] field, (int x, int y) position)
{
    return 0 <= position.x && position.x < field.GetLength(1) &&
        0 <= position.y && position.y < field.GetLength(0);
}

List<(int x, int y)> GetNeighbours(char[,] field, (int x, int y) position)
{
    var candidates = new List<(int x, int y)>
    {
        (position.x - 1, position.y - 1),
        (position.x, position.y - 1),
        (position.x + 1, position.y - 1),
        (position.x - 1, position.y),
        (position.x + 1, position.y),
        (position.x - 1, position.y + 1),
        (position.x, position.y + 1),
        (position.x + 1, position.y + 1),
    };

    return candidates.Where(p => IsValid(field, p)).ToList();
}

char[,] Round(char[,] field)
{
    var result = new char[field.GetLength(0), field.GetLength(1)];

    for (int row = 0; row < result.GetLength(0); row++)
    {
        for (int column = 0; column < result.GetLength(1); column++)
        {
            var neighbourValues = GetNeighbours(field, (column, row)).Select(p => field[p.y, p.x]).ToList();

            var trees = neighbourValues.Count(v => v == TREE);
            var lumberyards = neighbourValues.Count(v => v == LUMBERYARD);

            switch (field[row, column])
            {
                case OPEN:
                    result[row, column] = trees >= 3 ? TREE : OPEN;
                    break;
                case TREE:
                    result[row, column] = lumberyards >= 3 ? LUMBERYARD : TREE;
                    break;
                case LUMBERYARD:
                    result[row, column] = trees >= 1 && lumberyards >= 1 ? LUMBERYARD : OPEN;
                    break;
            }
        }
    }

    return result;
}

int Simulate(string[] input, int rounds)
{
    var past = new Dictionary<char[,], int>(new FieldEqualityComparer());

    var field = Parse(input);

    for (int round = 1; round <= rounds; round++)
    {
        field = Round(field);

        if (past != null)
        {
            if (past.ContainsKey(field))
            {
                var period = round - past[field];
                var fullPeriodsLeft = (rounds - round) / period;
                round += fullPeriodsLeft * period;

                past = null;

                continue;
            }

            past[field] = round;
        }
    }

    var trees = field.OfType<char>().Count(v => v == TREE);
    var lumberyards = field.OfType<char>().Count(v => v == LUMBERYARD);

    return trees * lumberyards;
}

class FieldEqualityComparer : IEqualityComparer<char[,]>
{
    public bool Equals(char[,]? x, char[,]? y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        for (int row = 0; row < x.GetLength(0); row++)
        {
            for (int column = 0; column < x.GetLength(1); column++)
            {
                if (x[row, column] != y[row, column])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int GetHashCode([DisallowNull] char[,] obj)
    {
        var hashCode = new HashCode();

        for (int row = 0; row < obj.GetLength(0); row++)
        {
            hashCode.Add(obj[row, 0]);
        }

        for (int column = 0; column < obj.GetLength(1); column++)
        {
            hashCode.Add(obj[0, column]);
        }

        return hashCode.ToHashCode();
    }
}
