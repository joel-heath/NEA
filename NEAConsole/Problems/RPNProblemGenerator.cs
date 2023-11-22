using NEAConsole.Numeracy;
using System.Text;

namespace NEAConsole.Problems;

public class RPNProblemGenerator(IRandom randomNumberGenerator) : IProblemGenerator
{
    public string DisplayText => "Reverse Polish Notation";
    public string SkillPath => "RPN";
    private readonly IRandom random = randomNumberGenerator;

    private static readonly char[] operators = ['+', '-', '×', '÷'];

    public IProblem Generate(Skill knowledge)
    {
        StringBuilder infix = new();

        var tokens = 2 * random.Next(2, 4);
        int digits;
        for (int i = 0; i < tokens; i++)
        {
            if (i % 2 == 0)
            {
                digits = random.NextDouble() < 0.7 ? 1 : 2;
                for (int j = 0; j < digits; j++)
                {
                    infix.Append(random.Next(0, 10));
                }
            }
            else
            {
                infix.Append(operators[random.Next(0, operators.Length)]);
            }

            infix.Append(' ');
        }

        digits = random.NextDouble() < 0.7 ? 1 : 2;
        for (int j = 0; j < digits; j++)
        {
            infix.Append(random.Next(0, 10));
        }

        var infixString = infix.ToString();

        Queue<ShuntingYard.Token> postfix = ShuntingYard.InfixToPostfix(ShuntingYard.Token.Tokenize(infixString));

        return new RPNProblem(infixString, string.Join(' ', postfix.ToArray()));
    }

    public RPNProblemGenerator() : this(new Random()) { }
}