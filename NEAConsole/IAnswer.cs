using NEAConsole.Matrices;

namespace NEAConsole;
public interface IAnswer { }
public class IntAnswer : IAnswer
{
    public int Answer { get; set; }
    public IntAnswer(int answer) { Answer = answer; }
}
public class DoubleAnswer : IAnswer
{
    public double Answer { get; set; }
    public DoubleAnswer(double answer) { Answer = answer; }
}
public class StringAnswer : IAnswer
{
    public string Answer { get; set; }
    public StringAnswer(string answer) { Answer = answer; }
}
public class MatrixAnswer : IAnswer
{
    public Matrix Answer { get; set; }
    public MatrixAnswer(Matrix answer) { Answer = answer; }
}
public class PrimsAnswer : IAnswer
{
    public HashSet<(int row, int col)> Answer { get; set; }
    public PrimsAnswer(HashSet<(int row, int col)> answer) { Answer = answer; }
}
public class ManyAnswer<T> : IAnswer
{
    public T[] Answer { get; set; }
    public ManyAnswer(T[] answer) { Answer = answer; }
}