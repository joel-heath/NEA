using NEAConsole.Problems;
using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly:InternalsVisibleTo("NEAConsoleTests")]

namespace NEAConsole;
internal class Program
{
    private const string USER_KNOWLEDGE_PATH = "UserKnowledge.json",
                         SAMPLE_KNOWLEDGE_PATH = "SampleKnowledge.json";

    /// <summary>
    /// If the user's knowledge tree is completely unknown, asks the user to update their knowledge tree, specifying which topics they know.
    /// </summary>
    /// <param name="knowledge">Skill representing knowledge tree.</param>
    /// <param name="quizTitle">Defaults to "random questions", will output "In order to use {quizTitle}, you must first enter the topics you know.</param>
    /// <returns>False if the user still doesn't know any topics, true if at least one topic is known.</returns>
    static bool TryUpdateKnowledge(Skill knowledge, string quizTitle = "random questions")
    {
        // or a while? NO, client chose to have it this way
        if (knowledge.Children.All(s => !s.Known))
        {
            Console.WriteLine($"To use {quizTitle}, you must first enter the topics you know.");
            UIMethods.Wait(string.Empty);
            Console.Clear();
            UpdateKnowledge(knowledge, false);

            if (knowledge.Children.All(s => !s.Known))
            {
                Console.WriteLine($"You cannot use {quizTitle} if you do not know any topics.");
                UIMethods.Wait(string.Empty);
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

        try { UIMethods.UpdateAllSkills(knowledge.Children); }
        catch (EscapeException)
        {
            knowledge.Children = oldSkills; // undo the resetting of knowledge
            Console.Clear();
            if (!catchEscape) throw new EscapeException();
        }

        File.WriteAllText(USER_KNOWLEDGE_PATH, JsonSerializer.Serialize(knowledge.Children));//, new JsonSerializerOptions { WriteIndented = true }));
    }

    static void SettingsMenu(Skill knowledge)
    {
        var options = new MenuOption[]
        {
            ("Update Knowledge", (k) => UpdateKnowledge(k, true)),
            ("Study Break Timer", (k) => Thread.Sleep(0)),
#if DEBUG
            ("Clear Knowledge", (k) =>
            {
                if (File.Exists(USER_KNOWLEDGE_PATH))
                    File.Delete(USER_KNOWLEDGE_PATH);
                k.ResetKnowledge(SAMPLE_KNOWLEDGE_PATH);
            })
#endif
        };

        Menu.ExecuteMenu(options, "Select a topic to revise", knowledge);
        Console.Clear();
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

    static void TopicMenu(Skill knowledge)
    {
        var options = new MenuOption[]
        {
            ("Maths", MathsMenu),
            ("Further Maths", FMathsMenu),
            ("Computer Science", CSciMenu),
        };

        Menu.ExecuteMenu(options, "Select a topic to revise", knowledge);
        Console.Clear();
    }

    static void QuickfireQuestions(Skill knowledge)
    {
        if (!TryUpdateKnowledge(knowledge)) return;

        var gen = new RandomProblemGenerator();
        bool @continue = true;
        while (@continue)
        {
            var problem = gen.Generate(knowledge);
            problem.Display();

            try
            {
                var answer = problem.GetAnswer();
                problem.Summarise(answer);
            }
            catch (EscapeException)
            {
                Console.Clear();
                @continue = false;
            }
            Console.Clear();
        }
    }

    static void MockExam(Skill knowledge)
    {
        //var skills = SelectSkills(knowledge.Traverse().ToList());
        var chosenKnowledge = Skill.KnowledgeConstructor(USER_KNOWLEDGE_PATH); // cheating way to make a deep copy of the user's knowledge object, just recreate from the file 

        UIMethods.UpdateKnownSkills(chosenKnowledge.Children);

        if (chosenKnowledge.Children.All(s => !s.Known))
        {
            Console.WriteLine($"You cannot start a mock exam without any topics.");
            UIMethods.Wait(string.Empty);
            Console.Clear();
            return;
        }

        Console.Write("How many questions would you like in the mock exam? ");
        var n = UIMethods.ReadInt();
        
        var gen = new RandomProblemGenerator();
        var attempts = Enumerable.Range(0, n).Select(i => ((IProblem problem, IAnswer? answer))(gen.Generate(chosenKnowledge), null)).ToList();
        int question = 1;

        UIMethods.Wait("Press any key to begin the exam...");
        Console.Clear();
        while (question < attempts.Count + 1) // needs to become until time is up
        {
            Console.WriteLine('[' + new string('#', question) + new string(' ', n-question) + ']'); // progress bar
            Console.WriteLine($"<- Question {question}/{n} ->");
            var (p, a) = attempts[question-1];
            p.Display();
            try
            {
                a = p.GetAnswer(a);
                attempts[question-1] = (p, a);
                question++;
            }
            catch (EscapeException)
            {
                try
                {
                    var choice = Menu.ExamMenu(new string[] { "<-", $"Question {question}/{n}", "->" }, question);
                    question += choice;
                }
                catch (EscapeException e)
                {
                    Console.Clear();
                    Console.WriteLine("Are you sure you want to exit the exam without finishing?");
                    if (Menu.Affirm()) throw e;
                }
            }

            Console.Clear();

            if (question == attempts.Count + 1)
            {
                Console.WriteLine("Are you sure you want to finish the exam early?");
                var unanswered = attempts.Select((a, i) => (a, i)).Where(a => a.a.answer is null);
                foreach (var att in unanswered)
                {
                    Console.WriteLine($"WARNING: Question {att.i + 1} is unanswered.");
                }
                if (!Menu.Affirm())
                {
                    question--;
                    Console.Clear();
                }
            }
        }

        Console.WriteLine($"Exam complete.");
        UIMethods.Wait();
        Console.Clear();

        question = 1;
        while (question < attempts.Count + 1) // needs to become until time is up
        {
            Console.WriteLine('[' + new string('#', question) + new string(' ', n - question) + ']'); // progress bar
            Console.WriteLine($"<- Question {question}/{n} ->");
            var (p, a) = attempts[question - 1];
            p.Display();
            try
            {
                if (a is not null) p.DisplayAnswer(a);
                else Console.WriteLine("You did not enter an answer."); 
                p.Summarise(a);

                var choice = Menu.ExamMenu(new string[] { "<-", $"Question {question}/{n}", "->" }, question);
                question += choice;
            }
            catch (EscapeException e)
            {
                Console.Clear();
                Console.WriteLine("Are you sure you want to finish reviewing the exam?");
                if (Menu.Affirm()) throw e;
            }

            if (question == attempts.Count + 1)
            {
                Console.Clear();
                Console.WriteLine("Are you sure you want to finish reviewing the exam?");
                if (!Menu.Affirm()) question--;
            }

            Console.Clear();
        }
    }

    static void Main(string[] args)
    {
        Skill knowledge = Skill.KnowledgeConstructor(File.Exists(USER_KNOWLEDGE_PATH) ? USER_KNOWLEDGE_PATH : SAMPLE_KNOWLEDGE_PATH);
        
        var options = new MenuOption[]
        {
            ("Mock Exam", MockExam),
            ("Quickfire Questions", QuickfireQuestions),
            ("Topic Select", TopicMenu),
            ("Settings", SettingsMenu),
        };

        Menu.ExecuteMenu(options, "Main Menu", knowledge);
        Console.Clear();
    }
}