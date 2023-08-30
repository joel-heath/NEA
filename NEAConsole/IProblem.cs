namespace NEAConsole;
public class NotAnsweredException : Exception
{
    public NotAnsweredException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    public NotAnsweredException(string? message, Exception? innerException) : base(message, innerException) { }
    public NotAnsweredException(string? message) : base(message) { }
    public NotAnsweredException() { }
}
public interface IProblem
{
    void Display(); // Display the problem and - pure UI
    void GetAnswer(); // Get the user's attempt at answering - pure UI
    bool EvaluateAnswer(); // Check if the user is correct - pure logic, unit tests      THROWS NotAnsweredException if called before GetAnswer is called
    void Summarise(); // Tell the user if they were right or wrong - pure UI             THROWS NotAnsweredException if called before GetAnswer is called
}
public interface IProblemGenerator
{
    string DisplayText { get; }
    /// <summary>
    /// Knowledge tree path to skill e.g. Matrices.Determinants.Inversion
    /// </summary>
    string SkillPath { get; }
    IProblem Generate(); // pure logic - unit tests
}