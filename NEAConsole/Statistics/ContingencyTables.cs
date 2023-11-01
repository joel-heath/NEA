namespace NEAConsole.Statistics;
public static class ContingencyTables
{
    public static double CalculateChiSquared(double[,] fo)
    {
        var n = fo.GetLength(0); // ROWS
        var m = fo.GetLength(1); // COLS
        var fe = FindExpectedFrequencies(fo, n, m);

        double chi2 = 0;

        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < m; c++)
            {
                var difference = fo[r, c] - fe[r, c];
                chi2 += difference * difference / fe[r, c];
            }
        }

        return chi2;
    }

    private static double[,] FindExpectedFrequencies(double[,] fo, int n, int m)
    {
        var fe = new double[n + 1, m + 1];

        // ROW SUMS (sum up the values across each row in one specific column, store in the (n+1)th item (nth index) of that column)
        for (int c = 0; c < n; c++)
        {
            double sum = 0;
            for (int r = 0; r < m; r++)
            {
                sum += fo[r, c];
            }
            fe[n, c] = sum;
        }

        // COL SUMS
        for (int r = 0; r < n; r++)
        {
            double sum = 0;
            for (int c = 0; c < m; c++)
            {
                sum += fo[r, c];
            }
            fe[r, m] = sum;
        }

        // SUM OF SUMS
        double superSum = 0;
        for (int c = 0; c < m; c++)
        {
            superSum += fe[n, c];
        }
        fe[n, m] = superSum;


        // EXPECTED FREQUENCES
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < m; c++)
            {
                fe[r, c] = fe[r, n] * fe[m, c] / fe[n, m];
            }
        }

        /// LESS THAN 5 GROUPINGS

        return fe;
    }
}