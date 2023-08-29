using NEAConsole.Problems;
using System.Text.Json.Nodes;

namespace NEAConsole;
public class Program
{
    private static Knowledge? Knowledge;
    static void GenericMenu(IEnumerable<IProblemGenerator> options, string prompt)
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
    static bool Affirm()
    {
        int choice = Menu.Choose(new MenuOption[] { ("Yes", null!), ("No", null!) });
        return choice == 0;
    }

    static void InitialiseSkills()
    {
        var matrices = new Skill("Matrices", new()
        {
            new Skill("Addition"),
            new Skill("Multiplication"),
            new Skill("Determinants", new()
            {
                new Skill("Minors", new()
                {
                    new Skill("Inversion")
                })
            }),
        }, new MatricesProblemGenerator());

        var graphs = new Skill("Graph Theory", new()
        {
            new Skill("Prim's Algorithm", new(), new PrimsProblemGenerator()),
            new Skill("Dijkstra's Algorithm", new(), new DijkstrasProblemGenerator()),
        });

        var simplex = new Skill("The Simplex Method", new(), new SimplexProblemGenerator());
        //var rpn = new Skill("Reverse Polish Notation", new());
        //var hypothesis = new Skill("Hypothesis Testing", new());

        Knowledge = new Knowledge(matrices, graphs, simplex);
    }

    static IEnumerable<IProblemGenerator> CreateAskableList(IList<Skill> skills)
    {
        foreach (Skill skill in skills)
        {
            if (!skill.Known)
            {
                continue;
            }

            if (skill.ProblemGenerator is not null)
            {
                yield return skill.ProblemGenerator;
            }

            if (skill.Children.Count > 0)
            {
                foreach (var problemGenerator in CreateAskableList(skill.Children))
                {
                    yield return problemGenerator;
                }
            }
        }
    }

    static bool TryImportKnowledge()
    {
        try
        {
            using var br = new BinaryReader(new FileStream("knowledge.dat", FileMode.Open));

            InitialiseSkills();

            Knowledge!.Matrices.Known = br.ReadBoolean();
            Knowledge.Matrices.Children[0].Known = br.ReadBoolean();
            Knowledge.Matrices.Children[1].Known = br.ReadBoolean();
            Knowledge.Matrices.Children[2].Known = br.ReadBoolean();
            Knowledge.Matrices.Children[2].Children[0].Known = br.ReadBoolean();
            Knowledge.Matrices.Children[2].Children[0].Children[0].Known = br.ReadBoolean();

            Knowledge.Graphs.Known = br.ReadBoolean();
            Knowledge.Graphs.Children[0].Known = br.ReadBoolean();
            Knowledge.Graphs.Children[1].Known = br.ReadBoolean();

            Knowledge.Simplex.Known = br.ReadBoolean();
            return true;
        }
        catch (FileNotFoundException)
        {
            Knowledge = null;
            return false;
        }
        catch (EndOfStreamException)
        {
            Knowledge = null;
            return false;
        }
    }

    static void UserUpdateSkills(IList<Skill> skills, IEnumerable<string>? skillPath = null)
    {
        skillPath ??= new List<string>();
        foreach (Skill skill in skills)
        {
            var newPath = skillPath.Append(skill.Name);
            Console.WriteLine($"Do you know {string.Join(" > ", newPath)}?");

            bool response = Affirm();
            Console.Clear();

            if (!response) continue;

            skill.Known = true;
            if (skill.Children.Count > 0)
            {
                UserUpdateSkills(skill.Children, newPath);
            }
        }
    }

    static void UpdateKnowledge()
    {
        UserUpdateSkills(Knowledge!.AsArray);

        using var bw = new BinaryWriter(new FileStream("knowledge.dat", FileMode.Create));

        bw.Write(Knowledge.Matrices.Known);
        bw.Write(Knowledge.Matrices.Children[0].Known);
        bw.Write(Knowledge.Matrices.Children[1].Known);
        bw.Write(Knowledge.Matrices.Children[2].Known);
        bw.Write(Knowledge.Matrices.Children[2].Children[0].Known);
        bw.Write(Knowledge.Matrices.Children[2].Children[0].Children[0].Known);

        bw.Write(Knowledge.Graphs.Known);
        bw.Write(Knowledge.Graphs.Children[0].Known);
        bw.Write(Knowledge.Graphs.Children[1].Known);

        bw.Write(Knowledge.Simplex.Known);
    }

    static void RandomQuestionTest()
    {
        if (Knowledge is null)
        {
            Console.WriteLine("To use random questions, you must first enter the topics you know.");
            Console.ReadKey(false);
            InitialiseSkills();
            UpdateKnowledge();
        }

        var generators = CreateAskableList(Knowledge!.AsArray).ToList();
        var rand = new Random();

        for (int i = 0; i < 10; i++)
        {
            var gen = generators[rand.Next(generators.Count)];

            var problem = gen.Generate();

            problem.Display();
            problem.GetAnswer();
            problem.Summarise();

            Console.WriteLine($"Continue?");
            if (!Affirm())
            {
                break;
            }
            Console.Clear();
        }
    }

    static void MathsMenu()
    {

    }
    static void FMathsMenu()
    {
        IProblemGenerator[] options = { new MatricesProblemGenerator(), new SimplexProblemGenerator(), new PrimsProblemGenerator(), new DijkstrasProblemGenerator() }; // Hypothesis Testing

        GenericMenu(options, "Choose a subject to revise");
        Console.Clear();
    }

    static void CSciMenu()
    {

    }

    static void Main(string[] args)
    {
        TryImportKnowledge();

        var options = new MenuOption[]
        {
            ("Maths", MathsMenu),
            ("Further Maths", FMathsMenu),
            ("Computer Science", CSciMenu),
            ("Random Questions", RandomQuestionTest),
            ("Update Knowledge", UpdateKnowledge)
        };

        GenericMenu(options, "Main Menu");
        Console.Clear();
    }
}