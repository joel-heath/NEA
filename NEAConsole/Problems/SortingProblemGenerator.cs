namespace NEAConsole.Problems;

public enum SortType { Quicksort, MergeSort, BubbleSort }

public class SortingProblemGenerator
{
    private readonly IRandom random;
    private readonly SortType sortType;

    protected IProblem Generate()
    {
        bool isAscending = random.NextDouble() < 0.5;

        int n = random.Next(5, 8);

        var data = new int[n];

        for (int i = 0; i < n; i++)
        {
            data[i] = random.Next(2, 20);
        }

        var sorted = sortType switch
        {
            SortType.Quicksort => data.QuickSort(),
            SortType.MergeSort => data.MergeSort(),
            SortType.BubbleSort => data.BubbleSort(),
            _ => throw new InvalidOperationException()
        };

        return new SortingProblem(data, isAscending ? sorted.ToArray() : sorted.Reverse().ToArray(), isAscending, sortType);
    }

    public SortingProblemGenerator(IRandom randomNumberGenerator, SortType sortType)
        => (random, this.sortType) = (randomNumberGenerator, sortType);
}

public class QuicksortProblemGenerator(IRandom randomNumberGenerator) : SortingProblemGenerator(randomNumberGenerator, SortType.Quicksort), IProblemGenerator
{
    public string DisplayText => "Quicksort";
    public string SkillPath => "Sorting.Quicksort";

    public IProblem Generate(Skill knowledge) => Generate();

    public QuicksortProblemGenerator() : this(new Random()) { }
}

public class MergeSortProblemGenerator(IRandom randomNumberGenerator) : SortingProblemGenerator(randomNumberGenerator, SortType.MergeSort), IProblemGenerator
{
    public string DisplayText => "Merge Sort";
    public string SkillPath => "Sorting.Merge Sort";

    public IProblem Generate(Skill knowledge) => Generate();

    public MergeSortProblemGenerator() : this(new Random()) { }
}

public class BubbleSortProblemGenerator(IRandom randomNumberGenerator) : SortingProblemGenerator(randomNumberGenerator, SortType.BubbleSort), IProblemGenerator
{
    public string DisplayText => "Bubble Sort";
    public string SkillPath => "Sorting.Bubble Sort";

    public IProblem Generate(Skill knowledge) => Generate();

    public BubbleSortProblemGenerator() : this(new Random()) { }
}