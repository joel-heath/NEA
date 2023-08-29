namespace NEAConsole;
public class NotAnsweredException : Exception
{
    public NotAnsweredException() { }
}
public interface IProblem
{
    void Display(); // Display the problem and -- pure UI
    void GetAnswer(); // Get the user's attempt at answering -- pure UI
    bool EvaluateAnswer(); // Check if the user is correct -- pure logic, unit tests      THROWS NotAnsweredException if called b4 GetAnswer is called
    void Summarise(); // Tell the user if they were right or wrong -- pure UI             THROWS NotAnsweredException if called b4 GetAnswer is called
}
public interface IProblemGenerator
{
    string DisplayText { get; }
    IProblem Generate(); // pure logic - unit tests
}