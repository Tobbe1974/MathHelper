using MathHelper.Models;

namespace MathHelper.Services;

public class MathProblem
{
    public int Operand1 { get; set; }
    public int Operand2 { get; set; }
    public MathCategory Category { get; set; }
    public int CorrectAnswer { get; set; }
    public string Display => Category switch
    {
        MathCategory.Addition => $"{Operand1} + {Operand2}",
        MathCategory.Subtraction => $"{Operand1} - {Operand2}",
        MathCategory.Multiplication => $"{Operand1} × {Operand2}",
        MathCategory.Division => $"{Operand1} ÷ {Operand2}",
        _ => throw new ArgumentOutOfRangeException()
    };
}

public class MathProblemService
{
    private readonly Random _random = new();

    public (int min, int max) GetRangeForLevel(int level)
    {
        return level switch
        {
            1 => (1, 5),
            2 => (1, 10),
            3 => (1, 15),
            4 => (1, 20),
            5 => (1, 25),
            6 => (1, 30),
            7 => (1, 40),
            8 => (1, 50),
            9 => (1, 75),
            10 => (1, 100),
            _ => (1, 100)
        };
    }

    public MathProblem GenerateProblem(MathCategory category, int level)
    {
        var (min, max) = GetRangeForLevel(level);

        return category switch
        {
            MathCategory.Addition => GenerateAddition(min, max),
            MathCategory.Subtraction => GenerateSubtraction(min, max),
            MathCategory.Multiplication => GenerateMultiplication(level),
            MathCategory.Division => GenerateDivision(level),
            _ => throw new ArgumentOutOfRangeException(nameof(category))
        };
    }

    private MathProblem GenerateAddition(int min, int max)
    {
        var a = _random.Next(min, max + 1);
        var b = _random.Next(min, max + 1);
        return new MathProblem
        {
            Operand1 = a,
            Operand2 = b,
            Category = MathCategory.Addition,
            CorrectAnswer = a + b
        };
    }

    private MathProblem GenerateSubtraction(int min, int max)
    {
        var a = _random.Next(min, max + 1);
        var b = _random.Next(min, max + 1);
        // Ensure result is non-negative
        if (b > a) (a, b) = (b, a);
        return new MathProblem
        {
            Operand1 = a,
            Operand2 = b,
            Category = MathCategory.Subtraction,
            CorrectAnswer = a - b
        };
    }

    private MathProblem GenerateMultiplication(int level)
    {
        // For multiplication, use smaller ranges to keep it reasonable
        var maxMultiplier = level switch
        {
            1 => 5,
            2 => 6,
            3 => 7,
            4 => 8,
            5 => 9,
            6 => 10,
            7 => 11,
            8 => 12,
            9 => 15,
            10 => 20,
            _ => 12
        };
        var a = _random.Next(1, maxMultiplier + 1);
        var b = _random.Next(1, maxMultiplier + 1);
        return new MathProblem
        {
            Operand1 = a,
            Operand2 = b,
            Category = MathCategory.Multiplication,
            CorrectAnswer = a * b
        };
    }

    private MathProblem GenerateDivision(int level)
    {
        // Generate division from multiplication to ensure whole number results
        var maxMultiplier = level switch
        {
            1 => 5,
            2 => 6,
            3 => 7,
            4 => 8,
            5 => 9,
            6 => 10,
            7 => 11,
            8 => 12,
            9 => 15,
            10 => 20,
            _ => 12
        };
        var b = _random.Next(1, maxMultiplier + 1);
        var answer = _random.Next(1, maxMultiplier + 1);
        var a = b * answer;
        return new MathProblem
        {
            Operand1 = a,
            Operand2 = b,
            Category = MathCategory.Division,
            CorrectAnswer = answer
        };
    }
}
