namespace NEAConsole.Statistics;

public readonly record struct SummaryStats
{
    public readonly int n;
    public readonly double Σx, Σy, Σx2, Σy2, Σxy;
    public readonly double x̄, ȳ;
    public readonly double σx, σy, σx2, σy2;
    public readonly double Sxx, Syy, Sxy;

    public SummaryStats(IEnumerable<double> xVals, IEnumerable<double> yVals) : this(xVals.Zip(yVals)) { }

    public SummaryStats(IEnumerable<(double x, double y)> data)
    {
        n = 0;
        Σx = 0;
        Σy = 0;
        Σx2 = 0;
        Σy2 = 0;
        Σxy = 0;

        foreach ((double x, double y) in data)
        {
            Σx += x;
            Σy += y;
            Σx2 += x * x;
            Σy2 += y * y;
            Σxy += x * y;
            n++;
        }

        x̄ = Σx / n;
        ȳ = Σy / n;

        σx2 = Σx2 / n - x̄ * x̄;
        σy2 = Σy2 / n - ȳ * ȳ;
        σx = Math.Sqrt(σx2);
        σy = Math.Sqrt(σy2);

        Sxx = Σx2 - Σx * Σx / n;
        Syy = Σy2 - Σy * Σy / n;
        Sxy = Σxy - Σx * Σy / n;
    }
}
