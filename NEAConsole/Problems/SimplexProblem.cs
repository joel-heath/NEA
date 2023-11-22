using NEAConsole.Matrices;
using System.ComponentModel.Design;

namespace NEAConsole.Problems;

public class SimplexProblem : IProblem
{
    private readonly SimplexInequality objective;
    private readonly SimplexInequality[] constraints;
    private readonly int[] solution;

    public void Display()
    {
#if DEBUG
        DebugDisplay();
#else
        Console.WriteLine($"Maximise P = {objective.ToObjectiveString()}");
        Console.WriteLine($"Subject to:");

        for (int i = 0; i < constraints.Length; i++)
        {
            Console.WriteLine($"    {constraints[i]}");
        }
#endif
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null) // would be nice to be able to navigate up and down like in a matrix
    {
        var answer = oldAnswer as ManyAnswer<int> ?? new ManyAnswer<int>(new int[solution.Length + 1]);

        string[] rawNums;
        if (oldAnswer is null)
        {
            DisplayFirstAnswer(answer.Answer.Length - 1);
            rawNums = Enumerable.Repeat(string.Empty, answer.Answer.Length).ToArray();
        }
        else
        {
            DisplayAnswer(oldAnswer);
            rawNums = answer.Answer.Take(answer.Answer.Length - 1).Select(n => n.ToString()).Prepend(answer.Answer.Last().ToString()).ToArray();
        }

        bool entering = true;
        int selected = 0;
        int pos = rawNums[selected].Length;
        int indent = Console.CursorLeft + 4; // total indent is intial indent + |P = | (4 characters)
        int yIndent = Console.CursorTop - rawNums.Length;

        while (entering)
        {
            Console.CursorTop = yIndent + selected;
            Console.CursorLeft = indent + pos;
            var k = InputMethods.ReadKey(true, ct);
            if (k.KeyChar >= '0' && k.KeyChar <= '9')
            {
                Console.Write(k.KeyChar);
                Console.Write(rawNums[selected][pos..]);
                rawNums[selected] = rawNums[selected][..pos] + k.KeyChar + rawNums[selected][pos..];
                pos++;
                continue;
            }
            switch (k.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (pos > 0) pos--;
                    break;
                case ConsoleKey.RightArrow:
                    if (pos < rawNums[selected].Length) pos++;
                    break;
                case ConsoleKey.UpArrow:
                    if (selected > 0)
                    {
                        selected--;
                        if (pos > rawNums[selected].Length) 
                            pos = rawNums[selected].Length;
                    }
                    break;
                case ConsoleKey.Tab:
                case ConsoleKey.DownArrow:
                    if (selected < rawNums.Length - 1)
                    {
                        selected++;
                        if (pos > rawNums[selected].Length)
                            pos = rawNums[selected].Length;
                    }
                    break;
                case ConsoleKey.Home:
                    pos = 0;
                    break;
                case ConsoleKey.End:
                    pos = rawNums[selected].Length;
                    break;

                case ConsoleKey.Delete:
                    if (pos < rawNums[selected].Length)
                    {
                        Console.Write(rawNums[selected][(pos + 1)..] + ' ');
                        rawNums[selected] = rawNums[selected][..pos] + rawNums[selected][(pos + 1)..];
                    }
                    break;
                case ConsoleKey.Backspace:
                    if (pos > 0)
                    {
                        Console.CursorLeft--;
                        Console.Write(rawNums[selected][pos--..] + ' ');
                        rawNums[selected] = rawNums[selected][..pos] + rawNums[selected][(pos + 1)..];
                    }
                    break;

                case ConsoleKey.Escape:
                    throw new EscapeException();

                case ConsoleKey.Enter:
                    if (rawNums.All(n => int.TryParse(n, out int num) && num != 0))
                    {
                        entering = false;
                    }
                    break;
            }
        }
        Console.CursorTop += rawNums.Length - selected;
        Console.WriteLine();

        // constant term is entered by user first, but is in array last... sighhhh
        return new ManyAnswer<int>(rawNums.Skip(1).Select(int.Parse).Append(int.Parse(rawNums.First())).ToArray());
    }

    private static void DisplayFirstAnswer(int count)
    {

        Console.WriteLine("P = ");
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine((char)('x' + i) + " = ");
        }
    }

    public void DisplayAnswer(IAnswer answer)
    {
        var attempt = (answer as ManyAnswer<int> ?? throw new InvalidOperationException()).Answer;
        Console.WriteLine("P = " + attempt[^1]);
        for (int i = 0; i < attempt.Length - 1; i++)
        {
            Console.WriteLine((char)('x' + i) + " = " + attempt[i]);
        }
    }

    public bool EvaluateAnswer(IAnswer answer)
    {
        var attempt = (answer as ManyAnswer<int> ?? throw new InvalidOperationException()).Answer;
        return attempt[solution.Length] == objective.Constant
               && !attempt.Take(solution.Length - 1).Where((n, i) => n != solution[i]).Any(); // Why not just attempt.All()? Because it doesn't have an (item, index) overload
    }

    public void Summarise(IAnswer? answer)
    {
        bool correct;
        try { correct = answer is not null && EvaluateAnswer(answer); }
        catch (InvalidOperationException) { correct = false; }
        if (correct)
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine("Incorrect, the correct answer was:");
            Console.WriteLine("P = " + objective.Constant);
            for (int i = 0; i < solution.Length; i++)
            {
                Console.WriteLine((char)('x' + i) + " = " + solution[i]);
            }
        }
    }

    /// <summary>
    /// Writes the linear program in common LP solver format
    /// </summary>
    /// <param name="m">Matrix to be drawn.</param>
    private void DebugDisplay()
    {
        for (int i = 0; i < solution.Length; i++)
        {
            Console.WriteLine($"var x{i+1} >= 0;");
        }
        
        Console.WriteLine($"maximize z: {objective.ToObjectiveString(true)};");

        for (int i = 0; i < constraints.Length; i++)
        {
            Console.WriteLine($"subject to c{i+1}: {constraints[i].ToDebugString()};");
        }

        Console.WriteLine("end;");
        Console.WriteLine();
    }

    public SimplexProblem(SimplexInequality objective, SimplexInequality[] constraints, int[] solution)
    {
        this.objective = objective;
        this.constraints = constraints;
        this.solution = solution;
    }
}