namespace NEAConsole.Tests;
internal static class ITestExtensions
{
    public static IEnumerable<MenuOption> ToMenuOptions(this IEnumerable<ITest> tests)
        => tests.Select(t => new MenuOption(t.DisplayText, t.Test));
}