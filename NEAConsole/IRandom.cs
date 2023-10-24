using System;

namespace NEAConsole;
public interface IRandom
{
    int Next();
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
    int NextNotZero();
    int NextNotZero(int maxValue);
    int NextNotZero(int minValue, int maxValue);
    double NextDouble();
}
public class Random : IRandom
{
    private readonly System.Random random;

    public int Next() => random.Next();
    public int Next(int maxValue) => random.Next(maxValue);
    public int Next(int minValue, int maxValue) => random.Next(minValue, maxValue);

    public int NextNotZero() => random.Next(1, int.MaxValue);
    public int NextNotZero(int maxValue) => random.Next(1, maxValue);
    public int NextNotZero(int minValue, int maxValue)
        => minValue > 0 || maxValue < 0
            ? random.Next(minValue, maxValue)
            : random.NextDouble() < 0.5
                ? random.Next(minValue, 0)
                : random.Next(1, maxValue);

    public double NextDouble() => random.NextDouble();

    public Random() : this(new System.Random()) { }
    public Random(int seed) : this(new System.Random(seed)) { }
    private Random(System.Random random)
    {
        this.random = random;
    }
}