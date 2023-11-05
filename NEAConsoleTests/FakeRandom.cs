using NEAConsole;

namespace NEAConsoleTests;

public class FakeRandom : IRandom
{
    private readonly Queue<int> ints;
    private readonly Queue<double> doubles;

    public int Next() => ints.Dequeue();
    public int Next(int maxValue) => ints.Dequeue();
    public int Next(int minValue, int maxValue) => ints.Dequeue();
    public double NextDouble() => doubles.Dequeue();

    public int NextNotZero() => Next();
    public int NextNotZero(int maxValue) => Next(maxValue);
    public int NextNotZero(int minValue, int maxValue) => Next(minValue, maxValue);

    public FakeRandom(IEnumerable<int> ints) : this(new Queue<int>(ints), new Queue<double>(0)) { }
    public FakeRandom(IEnumerable<double> doubles) : this(new Queue<int>(0), new Queue<double>(doubles)) { }
    public FakeRandom(IEnumerable<int> ints, IEnumerable<double> doubles) : this(new Queue<int>(ints), new Queue<double>(doubles)) { }
    public FakeRandom(Queue<int> ints, Queue<double> doubles)
    {
        this.ints = ints;
        this.doubles = doubles;
    }
}