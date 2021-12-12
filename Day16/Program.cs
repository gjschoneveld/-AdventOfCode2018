using System.Text.RegularExpressions;

class Sample
{
    public List<int> Instruction { get; set; } = new List<int>();

    public List<int> RegistersBefore { get; set; } = new List<int>();
    public List<int> RegistersAfter { get; set; } = new List<int>();

    public int Opcode => Instruction[0];
    public int A => Instruction[1];
    public int B => Instruction[2];
    public int C => Instruction[3];

    public bool BehavesLike(Func<List<int>, int, int, int> instr)
    {
        var registers = new List<int>(RegistersBefore);

        registers[C] = instr(registers, A, B);

        return registers.SequenceEqual(RegistersAfter);
    }

    public static List<int> ExtractNumbers(string line)
    {
        return new Regex(@"\d+", RegexOptions.Compiled)
            .Matches(line)
            .Select(m => int.Parse(m.Value))
            .ToList();
    }

    public static Sample Parse(string[] lines)
    {
        return new Sample
        {
            Instruction = lines[1].Split(' ').Select(int.Parse).ToList(),
            RegistersBefore = ExtractNumbers(lines[0]),
            RegistersAfter = ExtractNumbers(lines[2]),
        };
    }
}

class Program
{
    public static (List<Sample> samples, List<List<int>> program) Parse(string[] input)
    {
        int line;
        var samples = new List<Sample>();

        for (line = 0; line < input.Length; line += 4)
        {
            if (line >= input.Length || input[line] == "")
            {
                break;
            }

            samples.Add(Sample.Parse(input[line..(line + 3)]));
        }

        while (line < input.Length && input[line] == "")
        {
            line++;
        }

        if (line >= input.Length)
        {
            return (samples, new List<List<int>>());
        }

        var program = input[line..].Select(line => line.Split(' ').Select(int.Parse).ToList()).ToList();

        return (samples, program);
    }

    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        (var samples, var program) = Parse(input);

        var instructions = new List<Func<List<int>, int, int, int>>
        {
            (regs, A, B) => regs[A] + regs[B],
            (regs, A, B) => regs[A] + B,
            (regs, A, B) => regs[A] * regs[B],
            (regs, A, B) => regs[A] * B,
            (regs, A, B) => regs[A] & regs[B],
            (regs, A, B) => regs[A] & B,
            (regs, A, B) => regs[A] | regs[B],
            (regs, A, B) => regs[A] | B,
            (regs, A, B) => regs[A],
            (regs, A, B) => A,
            (regs, A, B) => A > regs[B] ? 1 : 0,
            (regs, A, B) => regs[A] > B ? 1 : 0,
            (regs, A, B) => regs[A] > regs[B] ? 1 : 0,
            (regs, A, B) => A == regs[B] ? 1 : 0,
            (regs, A, B) => regs[A] == B ? 1 : 0,
            (regs, A, B) => regs[A] == regs[B] ? 1 : 0
        };

        var answer1 = 0;

        var possibilities = new Dictionary<int, List<int>>();

        foreach (var sample in samples)
        {
            var indices = instructions
                .Select((instruction, index) => (index, instruction))
                .Where(x => sample.BehavesLike(x.instruction))
                .Select(x => x.index)
                .ToList();

            if (indices.Count >= 3)
            {
                answer1++;
            }

            if (!possibilities.ContainsKey(sample.Opcode))
            {
                possibilities[sample.Opcode] = indices;
                continue;
            }

            possibilities[sample.Opcode] = possibilities[sample.Opcode].Intersect(indices).ToList();
        }

        Console.WriteLine($"Answer 1: {answer1}");

        var map = new Dictionary<int, int>();

        while (possibilities.Count > 0)
        {
            var known = possibilities.First(kv => kv.Value.Count == 1);

            map[known.Key] = known.Value[0];

            foreach (var kv in possibilities)
            {
                kv.Value.Remove(map[known.Key]);
            }

            possibilities.Remove(known.Key);
        }

        var registers = new List<int> { 0, 0, 0, 0 };

        foreach (var instruction in program)
        {
            registers[instruction[3]] = instructions[map[instruction[0]]](registers, instruction[1], instruction[2]);
        }

        var answer2 = registers[0];
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
