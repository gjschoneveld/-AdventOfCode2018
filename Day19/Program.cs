class Instruction
{
    public string Opcode { get; set; } = "";
    public List<int> Operands { get; set; } = new List<int>();

    public int A => Operands[0];
    public int B => Operands[1];
    public int C => Operands[2];

    public static Instruction Parse(string line)
    {
        var parts = line.Split(' ');

        return new Instruction
        {
            Opcode = parts[0],
            Operands = parts[1..].Select(int.Parse).ToList()
        };
    }
}

class Program
{
    public static int ParseInstructionPointer(string line)
    {
        var parts = line.Split(' ');

        return int.Parse(parts[1]);
    }

    public static void RunProgram(List<Instruction> program, int ip, List<int> registers)
    {
        var instructions = new Dictionary<string, Func<List<int>, int, int, int>>
        {
            ["addr"] = (regs, A, B) => regs[A] + regs[B],
            ["addi"] = (regs, A, B) => regs[A] + B,
            ["mulr"] = (regs, A, B) => regs[A] * regs[B],
            ["muli"] = (regs, A, B) => regs[A] * B,
            ["banr"] = (regs, A, B) => regs[A] & regs[B],
            ["bani"] = (regs, A, B) => regs[A] & B,
            ["borr"] = (regs, A, B) => regs[A] | regs[B],
            ["bori"] = (regs, A, B) => regs[A] | B,
            ["setr"] = (regs, A, B) => regs[A],
            ["seti"] = (regs, A, B) => A,
            ["gtir"] = (regs, A, B) => A > regs[B] ? 1 : 0,
            ["gtri"] = (regs, A, B) => regs[A] > B ? 1 : 0,
            ["gtrr"] = (regs, A, B) => regs[A] > regs[B] ? 1 : 0,
            ["eqir"] = (regs, A, B) => A == regs[B] ? 1 : 0,
            ["eqri"] = (regs, A, B) => regs[A] == B ? 1 : 0,
            ["eqrr"] = (regs, A, B) => regs[A] == regs[B] ? 1 : 0
        };

        while (0 <= registers[ip] && registers[ip] < program.Count)
        {
            var instruction = program[registers[ip]];

            registers[instruction.C] = instructions[instruction.Opcode](registers, instruction.A, instruction.B);
            registers[ip]++;
        }
    }

    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");

        var ip = ParseInstructionPointer(input[0]);
        var program = input[1..].Select(Instruction.Parse).ToList();

        var registers = new List<int> { 0, 0, 0, 0, 0, 0 };

        RunProgram(program, ip, registers);

        var answer1 = registers[0];
        Console.WriteLine($"Answer 1: {answer1}");


        registers = new List<int> { 1, 0, 0, 0, 0, 0 };

        RunProgram(program, ip, registers);

        var answer2 = registers[0];
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
