using NEAConsole.Problems;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NEAConsole;
public class Program
{
    private const string USER_KNOWLEDGE_PATH = "knowledge.dat",
                         SAMPLE_KNOWLEDGE_PATH = "Skills.dat";

    private static Knowledge Knowledge = new();
    static void GenericMenu(IEnumerable<IProblemGenerator> options, string prompt)
    => GenericMenu(options.Select(opt => opt.ToMenuOption()), prompt);
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

    static void UpdateSkills(IEnumerable<Skill> skills, IEnumerable<string>? skillPath = null)
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
            if (skill.Children.Length > 0)
            {
                UpdateSkills(skill.Children, newPath);
            }
        }
    }
    // update skills will just set Knowns, and search the whole tree. Update knowledge will initialise the knowledge object, update all skills, then save the file to the disk.

    static void UpdateKnowledge()
    {
        // create skill tree with only names, using defaults (LastRevised = DateTime.Min, Known = false)
        Skill[] skills = JsonSerializer.Deserialize<Skill[]>(File.ReadAllText(SAMPLE_KNOWLEDGE_PATH), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        Knowledge = new Knowledge(skills.First(s => s.Name == "Matrices"), skills.First(s => s.Name == "Graphs"), skills.First(s => s.Name == "Simplex"));

        // Get user to update Knowns for each skill
        UpdateSkills(Knowledge.AsArray);

        // Save to USER_KNOWLEDGE_PATH
        File.WriteAllText(USER_KNOWLEDGE_PATH, JsonSerializer.Serialize(Knowledge.AsArray));//, new JsonSerializerOptions { WriteIndented = true }));
    }

    static void RandomQuestionTest()
    {
        if (!Knowledge.Entered || Knowledge.AsArray.All(s => !s.Known))
        {
            Console.WriteLine("To use random questions, you must first enter the topics you know.");
            Console.ReadKey(false);
            Console.Clear();
            UpdateKnowledge();
        }

        var generators = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                            .Where(t => t.GetInterfaces().Contains(typeof(IProblemGenerator)))
                            .Select(t => (IProblemGenerator)Activator.CreateInstance(t)!)
                            .Where(g => Knowledge.IsKnown(g.SkillPath)).ToArray();


        var rand = new Random();
        for (int i = 0; i < 10; i++)
        {
            var gen = generators[rand.Next(generators.Length)];

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
        try
        {
            string jsonString = File.ReadAllText(USER_KNOWLEDGE_PATH);
            Skill[] skills = JsonSerializer.Deserialize<Skill[]>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            Knowledge = new Knowledge(skills.First(s => s.Name == "Matrices"), skills.First(s => s.Name == "Graphs"), skills.First(s => s.Name == "Simplex"));
        }
        catch (JsonException) { }
        catch (FileNotFoundException) { }

        var options = new MenuOption[]
        {
            ("Maths", MathsMenu),
            ("Further Maths", FMathsMenu),
            ("Computer Science", CSciMenu),
            ("Random Questions", RandomQuestionTest),
            ("Update Knowledge", UpdateKnowledge),
        };

        GenericMenu(options, "Main Menu");
        Console.Clear();
    }
}