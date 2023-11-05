namespace NEAConsole.Statistics;

public class Regression
{
    private class RankedItem // For Spearman's Rank
    {
        public readonly double x, y;
        public double xRank, yRank;

        public RankedItem(double x, double y, double xRank = -1, double yRank = -1)
        {
            (this.x, this.y, this.xRank, this.yRank) = (x, y, xRank, yRank);
        }

        public static implicit operator RankedItem((double x, double y) tuple) => new(tuple.x, tuple.y);
    };

    public SummaryStats SummaryStats { get; }
    public IList<(double x, double y)> Data { get; }

    /// <summary>
    /// Calculates the Person's Product-Moment Correlation Coefficient (r) value for a given set of data.
    /// </summary>
    /// <returns>The PMCC (-1 ≤ r ≤ 1).</returns>
    public double PMCC() => SummaryStats.Sxy / Math.Sqrt(SummaryStats.Sxx * SummaryStats.Syy);

    /// <summary>
    /// Calculates the Spearman's Rank Correlation Coefficient (ρ / rs) value for a given set of data.
    /// </summary>
    /// <returns>The ρ value (-1 ≤ ρ ≤ 1).</returns>
    public double SpearmansRank()
    {
        IEnumerable<RankedItem> items = Data.Select(d => (RankedItem)d).ToArray(); // .ToArray() IS ABSOLUTELY NECESSARY (Lazy evaluation, RankItems() includes a foreach)

        RankItems(items, i => i.x, (i, r) => i.xRank = r); // Rank x values
        RankItems(items, i => i.y, (i, r) => i.yRank = r); // Rank y values

        return 1 - 6 * items.Sum(r => (r.xRank - r.yRank) * (r.xRank - r.yRank)) / (SummaryStats.n * (SummaryStats.n * SummaryStats.n - 1));
    }

    /// <summary>
    /// Ranks a set of RankedItems from smallest to largest, either by the .x property or .y, as chosen by the selector, then sets that rank to the .xRank or .yRank, as chosen by the setter.
    /// </summary>
    /// <param name="items">The collection of RankedItems (all with rank -1) to be ranked (sorted)</param>
    /// <param name="selector">Decides whether RankedItem.x or RankedItem.y should be ranked (e.g. RankedItem => RankedItem.x)</param>
    /// <param name="setter">Decides whether RankedItem.xRank or RankedItem.yRank should be set (e.g. (RankedItem, rank) => RankedItem.y = rank)</param>
    private static void RankItems(IEnumerable<RankedItem> items, Func<RankedItem,double> selector, Action<RankedItem,double> setter)
    {
        var ordered = items.OrderBy(selector).GroupBy(selector); // (i => i.x) or (i => i.y);
        int rank = 0;
        foreach (var group in ordered)
        {
            int l = group.Count();
            double mean = 0.5 * (l + 2 * rank + 1);
            foreach (var item in group)
            {
                setter(item, mean); // item.xRank = mean; or item.yRank = mean;
            }
            rank += l;
        }
    }

    /// <summary>
    /// Calculates the Y on X Least Squares Regression Line.
    /// </summary>
    /// <returns>The gradient followed by the y-intercept of the line in a ValueTuple<double></returns>
    public (double m, double c) LeastSquaresYonX()
    {
        double b = SummaryStats.Sxy / SummaryStats.Sxx;
        double c = b * -SummaryStats.x̄ + SummaryStats.ȳ;

        return (b, c);
    }

    /// <summary>
    /// Calculates the X on Y Least Squares Regression Line.
    /// </summary>
    /// <returns>The gradient followed by the x-intercept of the line in a ValueTuple<double></returns>
    public (double m, double c) LeastSquaresXonY()
    {
        double bʹ = SummaryStats.Sxy / SummaryStats.Syy;
        double cʹ = bʹ * -SummaryStats.ȳ + SummaryStats.x̄;

        return (bʹ, cʹ);
    }

    public Regression(IEnumerable<double> xValues, IEnumerable<double> yValues) : this(xValues.Zip(yValues)) { }

    public Regression(IEnumerable<(double x, double y)> data)
    {
        Data = data.ToList();
        SummaryStats = new(Data);
    }
}