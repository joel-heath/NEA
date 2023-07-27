using NEAConsole.Tests;

namespace NEAConsole;
public class Program
{
    static void MathsMenu()
    {

    }
    static void FMathsMenu()
    {
        ITest[] options = { new MatricesTest(), new SimplexTest() }; //, ("Hypothesis Testing", CSciMenu), ("Dijkstra's", CSciMenu), ("Prim's", CSciMenu), ("Return", CSciMenu) };
        GenericMenu(options, "Choose a subject to revise");
        Console.Clear();
    }

    static void CSciMenu()
    {

    }

    static void Main(string[] args)
    {
        MenuOption[] options = { ("Maths", MathsMenu), ("Further Maths", FMathsMenu), ("Computer Science", CSciMenu) };
        GenericMenu(options, "Choose a subject to revise");
        Console.Clear();
    }

    static void GenericMenu(IEnumerable<ITest> options, string prompt)
        => GenericMenu(options.ToMenuOptions(), prompt);
    static void GenericMenu(IEnumerable<MenuOption> options, string prompt)
    {
        var choices = options.Append(("Exit", null!)).ToList();

        bool @continue = true;
        while (@continue)
        {
            Console.WriteLine(prompt);
            var choice = Menu.Choose(choices);
            if (choice == choices.Count - 1)
            { 
                @continue = false;
            }
            else
            {
                Console.Clear();
                choices[choice].OnSelected();
            }
        }
    }
}