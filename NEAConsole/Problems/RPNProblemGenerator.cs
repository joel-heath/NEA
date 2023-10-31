using JH24Utils.Numeracy;
using System.Text;

namespace NEAConsole.Problems;
internal class RPNProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Reverse Polish Notation";
    public string SkillPath => "RPN";
    private readonly IRandom random;

    private static readonly char[] operators = { '+', '-', '×', '÷' };

    public IProblem Generate(Skill knowledge)
    {
        StringBuilder infix = new();

        for (int i = 0; i < 2 * random.Next(2, 4) + 1; i++)
        {
            if (i % 2 == 0)
            {
                for (int j = 0; j < random.Next(1,2); j++)
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

        for (int j = 0; j < random.Next(0, 2); j++)
        {
            infix.Append(random.Next(0, 10));
        }

        var infixString = infix.ToString();

        Queue<ShuntingYard.Token> postfix = ShuntingYard.InfixToPostfix(ShuntingYard.Token.Tokenize(infixString));

        return new RPNProblem(infixString, string.Join(' ', postfix.ToArray()));
    }

    public RPNProblemGenerator() : this(new Random()) { }
    public RPNProblemGenerator(IRandom randomNumberGenerator) => random = randomNumberGenerator;
}