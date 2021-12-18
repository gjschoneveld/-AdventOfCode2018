[Flags]
enum Directions
{
    None = 0,
    North = 1,
    South = 2,
    East = 4,
    West = 8
}


class Program
{
    public static char PeekToken(string regex, ref int regexPosition)
    {
        return regex[regexPosition];
    }

    public static char GetToken(string regex, ref int regexPosition)
    {
        return regex[regexPosition++];
    }

    public static void ConsumeToken(string regex, ref int regexPosition)
    {
        regexPosition++;
    }

    public static void AddDoor(Dictionary<(int x, int y), Directions> doors, (int x, int y) position, Directions direction)
    {
        if (!doors.ContainsKey(position))
        {
            doors[position] = Directions.None;
        }

        doors[position] |= direction;
    }

    public static void AddDoors(Dictionary<(int x, int y), Directions> doors, Dictionary<(int x, int y), Directions> other, (int x, int y) offset)
    {
        foreach (var door in other)
        {
            AddDoor(doors, (door.Key.x + offset.x, door.Key.y + offset.y), door.Value);
        }
    }

    public static Directions GetDoor(Dictionary<(int x, int y), Directions> doors, (int x, int y) position)
    {
        if (doors.ContainsKey(position))
        {
            return doors[position];
        }

        return Directions.None;
    }

    public static (Dictionary<(int x, int y), Directions> doors, HashSet<(int x, int y)> positions) FindDoors(string regex, ref int regexPosition)
    {
        var doors = new Dictionary<(int x, int y), Directions>();

        var token = PeekToken(regex, ref regexPosition);
        var mapPosition = (x: 0, y: 0);

        while (token != '|' && token != ')' && token != '$')
        {
            if (token == '(')
            {
                var intermediatePositions = new HashSet < (int x, int y)>();
                var finalPositions = new HashSet<(int x, int y)>();

                while (token != ')')
                {
                    ConsumeToken(regex, ref regexPosition);

                    (var innerDoors, var innerPositions) = FindDoors(regex, ref regexPosition);

                    AddDoors(doors, innerDoors, mapPosition);
                    intermediatePositions.UnionWith(innerPositions.Select(p => (p.x + mapPosition.x, p.y + mapPosition.y)));

                    token = PeekToken(regex, ref regexPosition);
                }

                ConsumeToken(regex, ref regexPosition);

                (var remainderDoors, var remainderPositions) = FindDoors(regex, ref regexPosition);

                foreach (var position in intermediatePositions)
                {
                    AddDoors(doors, remainderDoors, position);
                    finalPositions.UnionWith(remainderPositions.Select(p => (p.x + position.x, p.y + position.y)));
                }

                return (doors, finalPositions);
            }

            switch (token)
            {
                case 'N':
                    AddDoor(doors, mapPosition, Directions.North);
                    mapPosition = (mapPosition.x, mapPosition.y - 1);
                    AddDoor(doors, mapPosition, Directions.South);
                    break;
                case 'S':
                    AddDoor(doors, mapPosition, Directions.South);
                    mapPosition = (mapPosition.x, mapPosition.y + 1);
                    AddDoor(doors, mapPosition, Directions.North);
                    break;
                case 'E':
                    AddDoor(doors, mapPosition, Directions.East);
                    mapPosition = (mapPosition.x + 1, mapPosition.y);
                    AddDoor(doors, mapPosition, Directions.West);
                    break;
                case 'W':
                    AddDoor(doors, mapPosition, Directions.West);
                    mapPosition = (mapPosition.x - 1, mapPosition.y);
                    AddDoor(doors, mapPosition, Directions.East);
                    break;
            }

            ConsumeToken(regex, ref regexPosition);
            token = PeekToken(regex, ref regexPosition);
        }

        return (doors, new HashSet<(int x, int y)> { mapPosition });
    }

    public static void Print(Dictionary<(int x, int y), Directions> doors)
    {
        var minX = doors.Min(d => d.Key.x);
        var maxX = doors.Max(d => d.Key.x);
        var minY = doors.Min(d => d.Key.y);
        var maxY = doors.Max(d => d.Key.y);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Console.Write("#");

                var door = GetDoor(doors, (x, y));

                Console.Write((door & Directions.North) > 0 ? "-" : "#");
            }

            Console.WriteLine("#");

            for (int x = minX; x <= maxX; x++)
            {
                var door = GetDoor(doors, (x, y));

                Console.Write((door & Directions.West) > 0 ? "|" : "#");

                Console.Write(x == 0 && y == 0 ? "X" : " ");
            }

            Console.WriteLine("#");

        }

        for (int x = minX; x <= maxX; x++)
        {
            Console.Write("##");
        }

        Console.WriteLine("#");
        Console.WriteLine();
    }

    public static List<((int x, int y) position, Directions direction)> GetNeighbours((int x, int y) position)
    {
        return new List<((int x, int y) position, Directions direction)>
        {
            ((position.x, position.y - 1), Directions.North),
            ((position.x, position.y + 1), Directions.South),
            ((position.x + 1, position.y), Directions.East),
            ((position.x - 1, position.y), Directions.West)
        };
    }

    public static void Main()
    {
        var input = File.ReadAllText("input.txt");

        var regexPosition = 0;
        (var doors, _) = FindDoors(input, ref regexPosition);

        //Print(doors);

        int steps = 0;
        int far = 0;

        var current = new List<(int x, int y)> { (0, 0) };
        var visited = new HashSet<(int x, int y)>();

        while (current.Count > 0)
        {
            steps++;

            visited.UnionWith(current);

            current = current
                .SelectMany(p => GetNeighbours(p).Where(nb => (doors[p] & nb.direction) > 0))
                .Select(nb => nb.position)
                .Where(p => !visited.Contains(p))
                .ToList();

            if (steps >= 1000)
            {
                far += current.Count;
            }
        }

        var answer1 = steps - 1;
        Console.WriteLine($"Answer 1: {answer1}");

        var answer2 = far;
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
