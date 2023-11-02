using NEAConsole.Problems;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("NEAConsoleTests")]

namespace NEAConsole;
internal class Program
{
    private const string USER_KNOWLEDGE_PATH = "UserKnowledge.json",
                         SAMPLE_KNOWLEDGE_PATH = "SampleKnowledge.json",
                         EXAM_PROFILES_PATH = "ExamProfiles.bin";

    /// <summary>
    /// If the user's context.Knowledge tree is completely unknown, asks the user to update their context.Knowledge tree, specifying which topics they know.
    /// </summary>
    /// <param name="context.Knowledge">Skill representing context.Knowledge tree.</param>
    /// <param name="quizTitle">Defaults to "random questions", will output "In order to use {quizTitle}, you must first enter the topics you know.</param>
    /// <returns>False if the user still doesn't know any topics, true if at least one topic is known.</returns>
    static bool TryUpdateKnowledge(Skill knowledge, string quizTitle = "use random questions")
    {
        // or a while? NO, client chose to have it this way
        if (knowledge.Children.All(s => !s.Known))
        {
            Console.WriteLine($"To {quizTitle} you must first enter the topics you know.");
            InputMethods.Wait(string.Empty);
            Console.Clear();
            UpdateKnowledge(knowledge, false);

            if (knowledge.Children.All(s => !s.Known))
            {
                Console.WriteLine($"You cannot {quizTitle} if you do not know any topics.");
                InputMethods.Wait(string.Empty);
                Console.Clear();
                return false;
            }
        }

        return true;
    }

    public static void UpdateKnowledge(Skill knowledge, bool catchEscape = true)
    {
        var oldSkills = knowledge.Children;
        knowledge.ResetKnowledge(SAMPLE_KNOWLEDGE_PATH);

        try { InputMethods.UpdateAllSkills(knowledge.Children); }
        catch (EscapeException)
        {
            knowledge.Children = oldSkills; // undo the resetting of context.Knowledge
            Console.Clear();
            if (!catchEscape) throw new EscapeException();
        }

        File.WriteAllText(USER_KNOWLEDGE_PATH, JsonSerializer.Serialize(knowledge.Children));//, new JsonSerializerOptions { WriteIndented = true }));
    }

    static void SettingsMenu(Context context)
    {
        var options = new MenuOption[]
        {
            ("Update Knowledge", (c) => UpdateKnowledge(c.Knowledge, true)),
            ("Study Break Timer", (c) => {
                Console.WriteLine($"Do you want to {(c.Timer.Enabled ? "dis" : "en")}able the timer? ");
                if (Menu.Affirm()) c.Timer.Enabled = !c.Timer.Enabled;
                Console.CursorTop += 2;
                Console.Write("How many minutes do you want to study for? ");
                c.Timer.StudyLength = TimeSpan.FromMinutes(InputMethods.ReadInt(startingNum:c.Timer.StudyLength.Minutes));
                Console.Write("How many minutes should the break be? ");
                c.Timer.BreakLength = TimeSpan.FromMinutes(InputMethods.ReadInt(startingNum:c.Timer.BreakLength.Minutes));
            }),
#if DEBUG
            ("Clear Knowledge", (c) =>
            {
                File.WriteAllBytes(USER_KNOWLEDGE_PATH, File.ReadAllBytes(SAMPLE_KNOWLEDGE_PATH));
                c.Knowledge.ResetKnowledge(SAMPLE_KNOWLEDGE_PATH);
            })
#endif
        };

        Menu.ExecuteMenu(options, "Settings", context);
        Console.Clear();
    }

    static void MathsMenu(Context context)
    {
        IProblemGenerator[] options = { new PMCCProblemGenerator(), new RegressionProblemGenerator() };

        Menu.ExecuteMenu(options, "Choose a subject to revise", context);
        Console.Clear();
    }

    static void MatricesMenu(Context context)
    {
        IProblemGenerator[] options = { new MatricesAdditionProblemGenerator(), new MatricesMultiplicationProblemGenerator(), new MatricesDeterminantsProblemGenerator(), new MatricesInversionProblemGenerator() }; // Hypothesis Testing

        Menu.ExecuteMenu(options, "Choose a subject to revise", context);
        Console.Clear();
    }

    static void FMathsMenu(Context context)
    {
        IProblemGenerator[] options = { new SimplexProblemGenerator(), new PrimsProblemGenerator(), new DijkstrasProblemGenerator(), new Chi2ProblemGenerator() };

        Menu.ExecuteMenu(options.Select(o => o.ToMenuOption()).Prepend(("Matrices", MatricesMenu)), "Choose a subject to revise", context);
        Console.Clear();
    }

    static void CSciMenu(Context context)
    {
        IProblemGenerator[] options = { new RPNProblemGenerator(), new QuicksortProblemGenerator(), new MergeSortProblemGenerator(), new BubbleSortProblemGenerator() };

        Menu.ExecuteMenu(options, "Choose a subject to revise", context);
        Console.Clear();
    }

    static void TopicMenu(Context context)
    {
        var options = new MenuOption[]
        {
            ("Maths", MathsMenu),
            ("Further Maths", FMathsMenu),
            ("Computer Science", CSciMenu)
        };

        Menu.ExecuteMenu(options, "Select a topic to revise", context);
        Console.Clear();
    }

    static void QuickfireQuestions(Context context)
    {
        if (!TryUpdateKnowledge(context.Knowledge)) return;

        var gen = new RandomProblemGenerator();
        bool @continue = true;
        while (@continue)
        {
            bool correct;
            var start = DateTime.Now;

            var problem = gen.GenerateNextBest(context.Knowledge);
            problem.Display();

            try
            {
                var answer = problem.GetAnswer();
                problem.Summarise(answer);
                correct = problem.EvaluateAnswer(answer);
                InputMethods.Wait();
                Console.Clear();
            }
            catch (EscapeException)
            {
                context.Timer.TimeSinceLastBreak += DateTime.Now - start;
                Console.Clear();
                @continue = false;
                return;
            }

            // this is very inefficient
            context.Knowledge.Query(((IProblemGenerator)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == problem.GetType().Name + "Generator"))!).SkillPath, out Skill? skill);
            (skill ?? throw new Exception()).LastRevised = DateTime.Now;
            if (correct) skill.TotalCorrect++;
            skill.TotalAttempts++;

            context.Timer.TimeSinceLastBreak += DateTime.Now - start;
            if (context.Timer.TimeForBreak) context.Timer.UseBreak();
        }
    }

    static Exam? OfferExamProfiles()
    {
        List<(string name, Exam? exam)> profiles = new();

        using var br = new BinaryReader(new FileStream(EXAM_PROFILES_PATH, FileMode.OpenOrCreate));
        while (br.BaseStream.Position < br.BaseStream.Length)
        {
            var name = br.ReadString();
            var questions = br.ReadInt32();
            var knowledge = Skill.KnowledgeConstructor(JsonSerializer.Deserialize<Skill[]>(br.ReadString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!);

            profiles.Add((name, new Exam(knowledge, questions)));
        }
        profiles.Add(("New profile", null));

        if (profiles.Count == 1) return null;
        
        Console.WriteLine("Choose an exam profile.");
        var choice = profiles[Menu.Choose(profiles.Select(p => (MenuOption)(p.name, null!)).ToArray())].exam;
        Console.CursorTop += profiles.Count;
        Console.WriteLine();

        return choice;
    }

    static void MockExam(Context context)
    {
        var exam = OfferExamProfiles();
        if (exam is null)
        {
            TryUpdateKnowledge(context.Knowledge, "start a mock exam");
            Skill? chosenKnowledge = Exam.CreateKnowledge(USER_KNOWLEDGE_PATH);
            if (chosenKnowledge is null) return;
            Console.Write("How many questions would you like in the mock exam? ");
            var questionCount = InputMethods.ReadInt();
            exam = new(chosenKnowledge, questionCount);

            Console.WriteLine("Would you like to save this exam profile?");
            if (Menu.Affirm())
            {
                Console.CursorTop += 2;
                Console.Write("Profile name: ");
                var name = InputMethods.ReadLine();

                using var br = new BinaryWriter(new FileStream(EXAM_PROFILES_PATH, FileMode.Append));
                br.Write(name);
                br.Write(questionCount);
                br.Write(JsonSerializer.Serialize(chosenKnowledge.Children));
            }

            Console.WriteLine();
        }
        
        var t = exam.Begin(context.Timer);
        try
        {
            t.Wait();
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is null) throw;
            throw ex.InnerException;
        }

        if (context.Timer.TimeForBreak) context.Timer.UseBreak();
    }

    static void Main()//string[] args)
    {
        if (!File.Exists(USER_KNOWLEDGE_PATH)) File.WriteAllBytes(USER_KNOWLEDGE_PATH, File.ReadAllBytes(SAMPLE_KNOWLEDGE_PATH));
        Context context = new(Skill.KnowledgeConstructor(USER_KNOWLEDGE_PATH), new());
        
        var options = new MenuOption[]
        {
            ("Mock Exam", MockExam),
            ("Quickfire Questions", QuickfireQuestions),
            ("Topic Select", TopicMenu),
            ("Settings", SettingsMenu),
        };

        Menu.ExecuteMenu(options, "Main Menu", context);
        Console.Clear();
    }
}