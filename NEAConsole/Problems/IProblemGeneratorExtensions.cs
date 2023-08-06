namespace NEAConsole.Problems;
internal static class IProblemGeneratorExtensions
{
    public static IEnumerable<MenuOption> ToMenuOptions(this IEnumerable<IProblemGenerator> problemGenerators)
        => problemGenerators.Select(pg => new MenuOption(pg.DisplayText, () =>
        {
            var problem = pg.Generate();
            problem.Display();
            problem.GetAnswer();
            problem.Summarise();
        }));
}