namespace NEAConsole;
public static class IProblemGeneratorExtensions
{
    public static MenuOption ToMenuOption(this IProblemGenerator problemGenerator) => new(problemGenerator.DisplayText, (context) =>
        {
            Console.Write("How many questions do you want to be tested on? ");
            int n = UIMethods.ReadInt();

            for (int i = 0; i < n; i++)
            {
                var problem = problemGenerator.Generate(context.Knowledge);
                problem.Display();
                var answer = problem.GetAnswer();
                problem.Summarise(answer);
                UIMethods.Wait();
                Console.Clear();

                if (context.Timer.TimeForBreak) context.Timer.UseBreak();
            }
        });
}