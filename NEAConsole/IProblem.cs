namespace NEAConsole;
public interface IProblem
{
    void Display(); // Display the problem and - pure UI
    //IAnswer GetAnswer(); // Get the user's attempt at answering - pure UI
    IAnswer GetAnswer(IAnswer? oldAnswer = null);
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