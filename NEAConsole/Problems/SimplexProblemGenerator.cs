namespace NEAConsole.Problems;
internal class SimplexProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Simplex";
    public string SkillPath => "Simplex";
    private readonly Random random;

    public IProblem Generate()
    {
        int dimensions = random.Next(2, 4);
        int[] solution = Enumerable.Range(0, dimensions).Select(n => random.Next(2, 6)).ToArray();

        var constraints = new SimplexInequality[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            constraints[i] = CreateConstraint(dimensions, solution);
        }

        // make sure objective is integers (make constraints divisible by dimensions)
        for (int i = 0; i < dimensions; i++)
        {
            var sum = 0;
            for (int j = 0; j < dimensions; j++)
            {
                sum += constraints[j].Coefficients[i];
            }
            var remainder = dimensions - (sum % dimensions); // need to make sure we have integer objective
                                                             // objective is average of constraints, therefore must be divisible by dimensions
                                                             // suppose sum of coefficients is 11 in a 3D LP. 11 % 3 == 2, 3 - 2 = 1, 11 + 1 = 12 now its divisible by 3.
            constraints[dimensions - 1].Coefficients[i] += remainder;
            constraints[dimensions - 1].Constant += remainder * solution[i];
        }

        SimplexInequality objective = GenerateObjectiveFunction(dimensions, solution, constraints);

        return new SimplexProblem(objective, constraints, solution);
    }

    private SimplexInequality CreateConstraint(int dimensions, int[] solution)
    {
        int[] coeffs = new int[dimensions];
        var constant = 0;
        for (int j = 0; j < dimensions; j++)
        {
            var coefficient = random.Next(1, 6);
            coeffs[j] = coefficient;
            constant += coefficient * solution[j];
        }

        return new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.LessThan);
    }

    private static SimplexInequality GenerateObjectiveFunction(int dimensions, int[] solution, IList<SimplexInequality> constraints)
    {
        int[] coeffs = new int[dimensions];
        var constant = 0;
        for (int i = 0; i < dimensions; i++)
        {
            int coefficient = 0;
            for (int j = 0; j < dimensions; j++)
            {
                coefficient += constraints[j].Coefficients[i];
            }

            if (coefficient % dimensions != 0) throw new Exception("Constraints are not divisible by the number of dimensions"); // this _should_ never happen
            coefficient /= dimensions;
            coeffs[i] = coefficient;
            constant += coefficient * solution[i];
        }

        return new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.LessThan);
    }

    public SimplexProblemGenerator() : this(new Random()) { }
    public SimplexProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}