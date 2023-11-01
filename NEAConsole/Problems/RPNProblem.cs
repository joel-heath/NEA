namespace NEAConsole.Problems;
internal class RPNProblem : IProblem
{
    private readonly string infix, postfix;

    public void Display()
    {
        Console.WriteLine("Convert the infix expression below into Reverse Polish Notation.\n");

        Console.WriteLine("Infix: " + infix);

        Console.Write("Postfix: ");
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null)
    {
        var answer = InputMethods.ReadLine(startingInput: (oldAnswer as StringAnswer)?.Answer, ct: ct).Trim().Replace('*', '×').Replace('x', '×').Replace('/', '÷');

        return new StringAnswer(answer);
    }

    public void DisplayAnswer(IAnswer answer)
        => Console.WriteLine((answer as StringAnswer ?? throw new InvalidOperationException()).Answer);

    public bool EvaluateAnswer(IAnswer answer)
        => (answer as StringAnswer ?? throw new InvalidOperationException()).Answer == postfix;

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
            Console.WriteLine($"Incorrect. The correct answer was: {postfix}.");
        }
    }

    public RPNProblem(string infix, string postfix)
    {
        this.infix = infix;
        this.postfix = postfix;
    }
}