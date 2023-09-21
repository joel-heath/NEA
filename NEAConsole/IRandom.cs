namespace NEAConsole;
public interface IRandom
{
    int Next();
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
    double NextDouble();
}
public class Random : IRandom
{
    private readonly System.Random random;

    public int Next() => random.Next();
    public int Next(int maxValue) => random.Next(maxValue);
    public int Next(int minValue, int maxValue) => random.Next(minValue, maxValue);
    public double NextDouble() => random.NextDouble();

    public Random() : this(new System.Random()) { }
    public Random(int seed) : this(new System.Random(seed)) { }
    private Random(System.Random random)
    {
        this.random = random;
    }
}