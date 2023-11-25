namespace NEAConsole.Problems;

public record class SimplexInequality
{
    public enum InequalityType { LessThan, GreaterThan, Equal }
    public int[] Coefficients { get; }
    public int Constant { get; set; }
    public InequalityType Inequality { get; }

    public SimplexInequality(int[] coeffs, int constant, InequalityType inequality)
    {
        Coefficients = coeffs;
        Constant = constant;
        Inequality = inequality;
    }

    /// <summary>
    /// MAXIMUM 3 VARIABLES (x, y, z)
    /// Does NOT simplify a + -b to a - b
    /// </summary>
    /// <returns></returns>
    public string ToDebugString() => string.Join(" + ", Coefficients.Select((c, i) => $"{c}*x{i + 1}"))
                                    + $" {(int)Inequality switch { 0 => "<=", 1 => ">=", _ => "=" }} {Constant}";
    public override string ToString()
    {
        string inequality = string.Empty;
        int firstCoefficient = 0;
        for (int i = 0; i < Coefficients.Length; i++)
        {
            if (Coefficients[i] == 0)
            {
                if (firstCoefficient > -1) firstCoefficient = i + 1;
                continue;
            }
            if (i == firstCoefficient)
            {
                firstCoefficient = -1;
            }
            else
            {
                inequality += Coefficients[i] > 0 ? " + " : " - ";
            }
            if (Coefficients[i] != 1)
            {
                inequality += Math.Abs(Coefficients[i]);
            }
            inequality += (char)('x' + i);
        }

        inequality += $" {(int)Inequality switch { 0 => "<=", 1 => ">=", _ => "=" }} {Constant}";

        return inequality;
    }
    public string ToObjectiveString(bool debugMode = false)
        => debugMode ? string.Join(" + ", Coefficients.Select((c, i) => $"{c}*x{i + 1}"))
                     : string.Join(" + ", Coefficients.Select((c, i) => $"{c}{(char)(i + 'x')}"));

}