// borrowed from: https://www.reddit.com/r/adventofcode/comments/a86jgt/comment/ec8vh2x/?utm_source=share&utm_medium=web2x&context=3
var input = File.ReadAllLines("input.txt");

var answer1 = Simulate(input, true);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = Simulate(input, false);
Console.WriteLine($"Answer 2: {answer2}");

int Simulate(string[] lines, bool part1)
{
    var reg = new int[] { 0, 0, 0, 0, 0, 0 };
    var boundedReg = int.Parse(lines[0][4..]);
    var instructions = new List<(string instruction, int a, int b, int c)>();
    lines = lines[1..];

    var seen = new HashSet<int>();
    int prev = 0;

    foreach (var line in lines)
    {
        var ops = line[5..].Split(' ');
        instructions.Add((line[..4], int.Parse(ops[0]), int.Parse(ops[1]), int.Parse(ops[2])));
    }

    var operations = new List<ops>()
    {
        seti,
        bani,
        eqri,
        addr,
        seti,
        seti,
        bori,
        seti,
        bani,
        addr,
        bani,
        muli,
        bani,
        gtir,
        addr,
        addi,
        seti,
        seti,
        addi,
        muli,
        gtrr,
        addr,
        addi,
        seti,
        addi,
        seti,
        setr,
        seti,
        eqrr,
        addr,
        seti
    };

    while (boundedReg < operations.Count)
    {
        operations[reg[boundedReg]](ref reg, instructions[reg[boundedReg]].a, instructions[reg[boundedReg]].b, instructions[reg[boundedReg]].c);
        reg[boundedReg]++;

        if (reg[boundedReg] == 28)
        {
            if (part1)
            {
                return reg[4];
            }

            if (seen.Contains(reg[4]))
            {
                return prev;
            }

            seen.Add(reg[4]);
            prev = reg[4];
        }
    }

    return prev;
}

void addr(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] + reg[b];

void addi(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] + b;

//void mulr(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] * reg[b];

void muli(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] * b;

//void banr(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] & reg[b];

void bani(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] & b;

//void borr(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] | reg[b];

void bori(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] | b;

void setr(ref int[] reg, int a, int b, int c) => reg[c] = reg[a];

void seti(ref int[] reg, int a, int b, int c) => reg[c] = a;

void gtir(ref int[] reg, int a, int b, int c) => reg[c] = a > reg[b] ? 1 : 0;

//void gtri(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] > b ? 1 : 0;

void gtrr(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] > reg[b] ? 1 : 0;

//void eqir(ref int[] reg, int a, int b, int c) => reg[c] = a == reg[b] ? 1 : 0;

void eqri(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] == b ? 1 : 0;

void eqrr(ref int[] reg, int a, int b, int c) => reg[c] = reg[a] == reg[b] ? 1 : 0;

delegate void ops(ref int[] reg, int a, int b, int c);
