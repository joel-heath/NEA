namespace NEAConsole.Problems;
internal static class IProblemGeneratorExtensions
{
    public static MenuOption ToMenuOption(this IProblemGenerator problemGenerator) => new(problemGenerator.DisplayText, () =>
        {
            var problem = problemGenerator.Generate();
            problem.Display();
            problem.GetAnswer();
            problem.Summarise();
        });
}