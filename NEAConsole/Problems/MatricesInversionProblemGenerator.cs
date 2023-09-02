using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesInversionProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Matrix Inversion";
    public string SkillPath => "Matrices.Determinants.Inversion";
    private readonly Random random;

    public IProblem Generate(Skill knowledge)
    {
        int dimension = random.Next(2, 4);

        Matrix mat = Matrix.Identity(dimension);

        // in order to get a matrix looking much different from the identity, we will need to do many operations
        // however if the scalars are large this will cause massive numbers in the matrix
        // so using a scalar greater than 2 or smaller than -2 should have a relatively low chance.

        for (int i = 0; i < 5 * (dimension - 1); i++)
        {
            //var sign = random.NextDouble() > 0.5 ? 1 : -1;
            //var scalar = sign * (random.NextDouble() > 0.8 ? random.Next(1, 4) : sign * random.Next(1, 3));
            var scalar = 1;

            var source = random.Next(0, dimension);
            var destination = RandomDestination(source, dimension); // cant have same source and destination
            if (random.NextDouble() > 0.5)
            {
                mat = mat.RowAddition(source, destination, scalar);
            }
            else
            {
                mat = mat.ColumnAddition(source, destination, scalar);
            }
        }

        if (random.NextDouble() > 0.5)
        {
            mat = mat.RowSwitch(random.Next(0, dimension), random.Next(0, dimension));
        }

        return new MatricesInversionProblem(mat, mat.Inverse);
    }

    public int RandomDestination(int source, int dimension)
    {
        int destination = source;
        while (source == destination)
        {
            destination = random.Next(0, dimension);
        }

        return destination;
    }

    public MatricesInversionProblemGenerator() : this(new Random()) { }
    public MatricesInversionProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}