using NEAConsole.Problems;
using System.Text.Json;

namespace NEAConsole;
public class Program
{
    private const string USER_KNOWLEDGE_PATH = "knowledge.dat",
                         SAMPLE_KNOWLEDGE_PATH = "Skills.dat";
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

    public static void UpdateKnowledge(Skill knowledge) => UpdateKnowledge(knowledge, true);
    public static void UpdateKnowledge(Skill knowledge, bool catchEscape)
    {
        var oldSkills = knowledge.Children; // so if they escape we can undo our resetting

        knowledge.ResetKnowledge(SAMPLE_KNOWLEDGE_PATH);
        try
        {
            UpdateSkills(knowledge.Children);
        }
        catch (EscapeException)
        {
            knowledge.Children = oldSkills; // undo the resetting of knowledge
            Console.Clear();
            if (!catchEscape) throw new EscapeException(); 
        }

        // Save to USER_KNOWLEDGE_PATH
        File.WriteAllText(USER_KNOWLEDGE_PATH, JsonSerializer.Serialize(knowledge.Children));//, new JsonSerializerOptions { WriteIndented = true }));
    }

    static void ClearKnowledge(Skill knowledge)
    {
        if (File.Exists(USER_KNOWLEDGE_PATH))
            File.Delete(USER_KNOWLEDGE_PATH);

        knowledge.ResetKnowledge(SAMPLE_KNOWLEDGE_PATH);
    }

    static void RandomQuestions(Skill knowledge)
    {
        // or a while? NO, client chose to have it this way
        if (knowledge.Children.All(s => !s.Known))
        {
            Console.WriteLine("To use random questions, you must first enter the topics you know.");
            UIMethods.Wait(string.Empty);
            Console.Clear();
            UpdateKnowledge(knowledge, false);

            if (knowledge.Children.All(s => !s.Known))
            {
                Console.WriteLine("You cannot use random questions if you do not know any topics.");
                UIMethods.Wait(string.Empty);
                Console.Clear();
                return;
            }
        }

        var gen = new RandomProblemGenerator();

        for (int i = 0; i < 10; i++)
        {
            var problem = gen.Generate(knowledge);

            problem.Display();
            try
            {
                problem.GetAnswer();
            }
            catch (EscapeException)
            {
                Console.Clear();
                return;
            }
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

    static void MathsMenu(Skill knowledge)
    {

    }
    static void FMathsMenu(Skill knowledge)
    {
        IProblemGenerator[] options = { new MatricesProblemGenerator(), new SimplexProblemGenerator(), new PrimsProblemGenerator(), new DijkstrasProblemGenerator() }; // Hypothesis Testing

        Menu.ExecuteMenu(options, "Choose a subject to revise", knowledge);
        Console.Clear();
    }

    static void CSciMenu(Skill knowledge)
    {

    }

    static void Main(string[] args)
    {
        Skill knowledge = Skill.KnowledgeConstructor(File.Exists(USER_KNOWLEDGE_PATH) ? USER_KNOWLEDGE_PATH : SAMPLE_KNOWLEDGE_PATH);
        
        var options = new MenuOption[]
        {
            ("Maths", MathsMenu),
            ("Further Maths", FMathsMenu),
            ("Computer Science", CSciMenu),
            ("Random Questions", RandomQuestions),
            ("Update Knowledge", UpdateKnowledge),
#if DEBUG
            
            ("Clear Knowledge", ClearKnowledge)
#endif
        };

        Menu.ExecuteMenu(options, "Main Menu", knowledge);
        Console.Clear();
    }
}