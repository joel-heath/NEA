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
    //public override string ToString() => string.Join(" + ", Coefficients.Select((c, i) => $"{c}{(char)(i + 'x')}"))
    //                                        + $" {(int)Inequality switch { 0 => "<=", 1 => ">=", _ => "=" }} {Constant}";
    public override string ToString()
    {
        string inequality = string.Empty;
        for (int i = 0; i < Coefficients.Length; i++)
        {
            if (Coefficients[i] == 0) continue;
            if (i != 0)
            {
                inequality += Coefficients[i] > 0 ? " + " : " - ";
            }
            inequality += Coefficients[i];
            inequality += (char)('x' + i);
        }

        inequality += $" {(int)Inequality switch { 0 => "<=", 1 => ">=", _ => "=" }} {Constant}";

        return inequality;
    }
    public string ToObjectiveString() => string.Join(" + ", Coefficients.Select((c, i) => $"{c}{(char)(i + 'x')}"));

}