namespace NEAConsole;

public interface IProblem
{
    void Display(); // Display the problem and - pure UI
    //IAnswer GetAnswer(); // Get the user's attempt at answering - pure UI
    /// <summary>
    /// Get user's attempt to the question through the console.
    /// </summary>
    /// <param name="oldAnswer">Preset answer to show when getting the user's answer, used in exams when going back to a previously attempted question.</param>
    /// <param name="ct">Cancellation token used for exiting early during multithreading.</param>
    /// <returns>An IAnswer containing the user's answer</returns>
    IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null);
    void DisplayAnswer(IAnswer answer);
    bool EvaluateAnswer(IAnswer answer); // Check if the user is correct - pure logic, unit tests
    void Summarise(IAnswer? answer); // Tell the user if they were right or wrong - pure UI
}

public interface IProblemGenerator
{
    /// <summary>
    /// Problem title e.g. Prim's Algorithm
    /// </summary>
    string DisplayText { get; }
    /// <summary>
    /// Knowledge tree path to skill e.g. Matrices.Determinants.Inversion
    /// </summary>
    string SkillPath { get; }
    IProblem Generate(Skill knowledge); // pure logic - unit tests
}