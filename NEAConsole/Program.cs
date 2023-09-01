using NEAConsole.Problems;
using System.Text.Json;

namespace NEAConsole;
public class Program
{
    private const string USER_KNOWLEDGE_PATH = "knowledge.dat",
                         SAMPLE_KNOWLEDGE_PATH = "Skills.dat";

    public static Knowledge Knowledge = new();

    static void UpdateSkills(IEnumerable<Skill> skills, IEnumerable<string>? skillPath = null)
    {
        skillPath ??= new List<string>();
        foreach (Skill skill in skills)
        {
            var newPath = skillPath.Append(skill.Name);
            Console.WriteLine($"Do you know {string.Join(" > ", newPath)}?");

            bool response = Menu.Affirm();
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

    public static void UpdateKnowledge()
    {
        // create skill tree with only names, using defaults (LastRevised = DateTime.Min, Known = false)
        Skill[] skills = JsonSerializer.Deserialize<Skill[]>(File.ReadAllText(SAMPLE_KNOWLEDGE_PATH), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        Knowledge = new Knowledge(skills.First(s => s.Name == "Matrices"), skills.First(s => s.Name == "Graphs"), skills.First(s => s.Name == "Simplex"));
        //Knowledge.Matrices = skills.First(s => s.Name == "Matrices");
        //Knowledge.Graphs = skills.First(s => s.Name == "Graphs");
        //Knowledge.Simplex = skills.First(s => s.Name == "Simplex");

        // Get user to update Knowns for each skill
        UpdateSkills(Knowledge.AsArray);

        // Save to USER_KNOWLEDGE_PATH
        File.WriteAllText(USER_KNOWLEDGE_PATH, JsonSerializer.Serialize(Knowledge.AsArray));//, new JsonSerializerOptions { WriteIndented = true }));
    }

    static void RandomQuestions()
    {
        if (!Knowledge.Entered || Knowledge.AsArray.All(s => !s.Known))
        {
            Console.WriteLine("To use random questions, you must first enter the topics you know.");
            Console.ReadKey(false);
            Console.Clear();
            UpdateKnowledge();

            if (!Knowledge.Entered || Knowledge.AsArray.All(s => !s.Known))
            {
                Console.WriteLine("You cannot use random questions if you do not know any topics");
                Console.ReadKey(false);
                Console.Clear();
                return;
            }
        }

        var gen = new RandomProblemGenerator();

        for (int i = 0; i < 10; i++)
        {
            var problem = gen.Generate();

            problem.Display();
            problem.GetAnswer();
            problem.Summarise();

            Console.WriteLine($"Continue?");
            if (!Menu.Affirm())
            {
                Console.Clear();
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

        Menu.ExecuteMenu(options, "Choose a subject to revise");
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
            ("Random Questions", RandomQuestions),
            ("Update Knowledge", UpdateKnowledge),
        };

        Menu.ExecuteMenu(options, "Main Menu");
        Console.Clear();
    }
}