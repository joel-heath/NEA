namespace NEAConsole.Tests;
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
    public override string ToString() => string.Join(" + ", Coefficients.Select((c, i) => $"{c}{(char)(i + 'x')}"))
                                            + $" {(int)Inequality switch { 0 => "<=", 1 => ">=", _ => "=" }} {Constant}";
    public string ToObjectiveString() => string.Join(" + ", Coefficients.Select((c, i) => $"{c}{(char)(i + 'x')}"));

}