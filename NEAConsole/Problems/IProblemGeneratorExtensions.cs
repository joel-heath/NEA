namespace NEAConsole.Problems;
internal static class IProblemGeneratorExtensions
{
    public static MenuOption ToMenuOption(this IProblemGenerator problemGenerator) => new(problemGenerator.DisplayText, (s) =>
        {
            var problem = problemGenerator.Generate(s);
            problem.Display();
            problem.GetAnswer();
            problem.Summarise();
        });
}