using NEAConsole.Matrices;

namespace NEAConsole;

public interface IAnswer { }
public class IntAnswer(int answer) : IAnswer
{
    public int Answer { get; set; } = answer;
}
public class DoubleAnswer(double answer) : IAnswer
{
    public double Answer { get; set; } = answer;
}
public class StringAnswer(string answer) : IAnswer
{
    public string Answer { get; set; } = answer;
}
public class MatrixAnswer(Matrix answer) : IAnswer
{
    public Matrix Answer { get; set; } = answer;
}
public class PrimsAnswer(HashSet<(int row, int col)> answer) : IAnswer
{
    public HashSet<(int row, int col)> Answer { get; set; } = answer;
}
public class ManyAnswer<T>(T[] answer) : IAnswer
{
    public T[] Answer { get; set; } = answer;
}