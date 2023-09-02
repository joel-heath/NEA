namespace NEAConsole.Problems;
internal static class IProblemGeneratorExtensions
{
    public static MenuOption ToMenuOption(this IProblemGenerator problemGenerator) => new(problemGenerator.DisplayText, (knowledge) =>
        {
            Console.Write("How many questions do you want to be tested on? ");
            int n = UIMethods.ReadInt();

            for (int i = 0; i < n; i++)
            {
                var problem = problemGenerator.Generate(knowledge);
                problem.Display();
                var answer = problem.GetAnswer();
                problem.Summarise(answer);
            }
        });
}