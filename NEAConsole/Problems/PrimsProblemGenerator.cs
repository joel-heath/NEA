using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class PrimsProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Prim's Algorithm";
    private readonly Random random;

    public IProblem Generate()
    {
        var dimension = random.Next(8, 11);
        Matrix tree = new(dimension);

        for (int i = 1; i < dimension; i++)
        {
            // pick one of the already connected vertices to connect this new one to
            var connector = random.Next(0, i);

            var weight = random.Next(1, 16);

            tree[connector, i] = weight;
            tree[i, connector] = weight;
        }

        // 5x^2 - 85x + 367   (see 2. Robert J. Prim's algorithm -- pg 10)
        var edgesToAdd = 5 * dimension * dimension - 85 * dimension + 367;

        for (int i = 0; i < edgesToAdd; i++)
        {
            int node1 = 0, node2 = 0;
            while (tree[node1, node2] != 0)
            {
                node1 = random.Next(0, dimension);
                node2 = random.Next(0, dimension);
            }
            var weight = random.Next(1, 16);

            if (tree[node1, node2] != 0 || tree[node2, node1] != 0) throw new Exception("Did not successfully choose nodes that weren't already connected");

            tree[node1, node2] = weight;
            tree[node2, node1] = weight;
        }

        var solution = MatrixUtils.Prims(tree.ToMatrix(e => e == 0 ? double.MaxValue : e)).ToHashSet();

        return new PrimsProblem(tree, solution);
    }

    public PrimsProblemGenerator() : this(new Random()) { }
    public PrimsProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}