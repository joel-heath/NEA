namespace NEAConsole;

public static class IProblemGeneratorExtensions
{
    public static MenuOption ToMenuOption(this IProblemGenerator problemGenerator) => new(problemGenerator.DisplayText, (context) =>
        {
            Console.Write("How many questions do you want to be tested on? ");
            int n = InputMethods.ReadInt();

            for (int i = 0; i < n; i++)
            {
                var start = DateTime.Now;
                var problem = problemGenerator.Generate(context.Knowledge);
                problem.Display();
                var answer = problem.GetAnswer();
                problem.Summarise(answer);
                InputMethods.Wait();
                Console.Clear();

                context.Timer.TimeSinceLastBreak += DateTime.Now - start;
                if (context.Timer.TimeForBreak) context.Timer.UseBreak();
            }
        });
}