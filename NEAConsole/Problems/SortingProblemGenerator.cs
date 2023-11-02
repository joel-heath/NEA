using System.ComponentModel;

namespace NEAConsole.Problems;
internal enum SortType
{
    [Description("Quicksort")]
    Quicksort,
    [Description("Merge Sort")]
    MergeSort,
    [Description("Bubble Sort")]
    BubbleSort
};
internal class SortingProblemGenerator
{
    private readonly IRandom random;
    private readonly SortType sortType;

    public IProblem Generate(Skill knowledge)
    {
        int n = random.Next(5, 8);

        var data = new int[n];

        for (int i = 0; i < n; i++)
        {
            data[i] = random.Next(2, 20);
        }

        return new SortingProblem(data, (sortType switch
        {
            SortType.Quicksort => data.QuickSort(),
            SortType.MergeSort => data.MergeSort(),
            SortType.BubbleSort => data.BubbleSort(),
            _ => throw new InvalidOperationException()
        }).ToArray(), sortType);
    }

    public SortingProblemGenerator(IRandom randomNumberGenerator, SortType sortType)
        => (random, this.sortType) = (randomNumberGenerator, sortType);
}

internal class QuicksortProblemGenerator : SortingProblemGenerator, IProblemGenerator
{
    public string DisplayText => "Quicksort";
    public string SkillPath => "Sorting.Quicksort";

    public QuicksortProblemGenerator() : this(new Random()) { }
    public QuicksortProblemGenerator(IRandom randomNumberGenerator) : base(randomNumberGenerator, SortType.Quicksort) { }
}

internal class MergeSortProblemGenerator : SortingProblemGenerator, IProblemGenerator
{
    public string DisplayText => "Merge Sort";
    public string SkillPath => "Sorting.Merge Sort";

    public MergeSortProblemGenerator() : this(new Random()) { }
    public MergeSortProblemGenerator(IRandom randomNumberGenerator) : base(randomNumberGenerator, SortType.MergeSort) { }
}

internal class BubbleSortProblemGenerator : SortingProblemGenerator, IProblemGenerator
{
    public string DisplayText => "Bubble Sort";
    public string SkillPath => "Sorting.Bubble Sort";

    public BubbleSortProblemGenerator() : this(new Random()) { }
    public BubbleSortProblemGenerator(IRandom randomNumberGenerator) : base(randomNumberGenerator, SortType.BubbleSort) { }
}