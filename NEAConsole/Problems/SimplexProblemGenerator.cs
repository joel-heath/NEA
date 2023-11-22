namespace NEAConsole.Problems;

public static class SimplexProblemGenerator
{
    public static IProblem Generate(bool twoStage, IRandom random)
    {
        int dimensions = random.Next(2, 4);
        int[] solution = Enumerable.Range(0, dimensions).Select(n => random.Next(2, 6)).ToArray();

        var constraints = new SimplexInequality[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            constraints[i] = CreateConstraint(dimensions, solution, random);
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

        // Two-stage! Will do this by generating a redundant contraint that takes the origin out the feasible region
        // so x >= 1, y >=2, z >= 6 etc. constant must be greater than 0 to actually take origin out, and must be less than or equal to
        // the solutions corresponding ordinate in order to not change the maximal solution.
        if (twoStage)
        {
            int variable = random.Next(0, dimensions);
            int[] coeffs = new int[dimensions];
            
            var coefficient = random.Next(1, 6);
            coeffs[variable] = coefficient;
            var maxConstant = coefficient * solution[variable];
            
            int constant = random.Next(1, maxConstant + 1);

            constraints = [.. constraints, new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.GreaterThan)];
        }

        SimplexInequality objective = GenerateObjectiveFunction(dimensions, solution, constraints);

        return new SimplexProblem(objective, constraints, solution);
    }

    private static SimplexInequality CreateConstraint(int dimensions, int[] solution, IRandom random)
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
}

public class OneStageSimplexProblemGenerator(IRandom randomNumberGenerator) : IProblemGenerator
{
    public string DisplayText => "One-stage Simplex";
    public string SkillPath => "Simplex.One-stage";
    private readonly IRandom random = randomNumberGenerator;

    public IProblem Generate(Skill knowledge)
        => SimplexProblemGenerator.Generate(false, random);

    public OneStageSimplexProblemGenerator() : this(new Random()) { }
}

public class TwoStageSimplexProblemGenerator(IRandom randomNumberGenerator) : IProblemGenerator
{
    public string DisplayText => "Two-stage Simplex";
    public string SkillPath => "Simplex.Two-stage";
    private readonly IRandom random = randomNumberGenerator;

    public IProblem Generate(Skill knowledge)
        => SimplexProblemGenerator.Generate(true, random);

    public TwoStageSimplexProblemGenerator() : this(new Random()) { }
}