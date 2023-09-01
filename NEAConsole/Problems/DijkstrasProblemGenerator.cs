using NEAConsole.Graphs;
using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class DijkstrasProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Dijkstra's";
    public string SkillPath => "Graphs.Dijkstras";
    private readonly Random random;

    public IProblem Generate()
    {
        var dimension = random.Next(8, 11);
        Matrix tree = new(dimension);

        for (int i = 1; i < dimension; i++)
        {
            // pick one of the already connected vertices to connect this new one to
            // would do 0 to i, but we want the final node to be far from the first node
            var connector = random.Next(Math.Max(0, i-3), i);

            var weight = random.Next(1, 13);

            tree[connector, i] = weight;
            tree[i, connector] = weight;
        }

        // populate graph a little more
        for (int i = 0; i < dimension - 5; i++)
        {
            int node1 = 0, node2 = 1;
            while (tree[node1, node2] != 0)
            {
                node1 = random.Next(0, dimension);
                node2 = SelectNodeToConnectTo(node1, dimension);
            }
            var weight = random.Next(13, 20);

            //if (tree[node1, node2] != 0 || tree[node2, node1] != 0) throw new Exception("Did not successfully choose nodes that weren't already connected");

            tree[node1, node2] = weight;
            tree[node2, node1] = weight;
        }

        // 0s mean no connection -- but the dijkstra's is naive and will take a connection weighted at 0, replace with infinities
        (var startingNode, var endingNode) = CreateGraph(tree);//tree.ToMatrix(e => e == 0 ? double.MaxValue : e));

        var distances = GraphUtils.Dijkstras(startingNode);

        return new DijkstrasProblem(tree, 'A', (char)('A' + tree.Rows - 1), distances[endingNode]);
    }

    public int SelectNodeToConnectTo(int connector, int dimension)
    {
        int connectend = connector;
        while (connector == connectend)
        { //                            if were on startign vertex, it may not connect to final vertex directly
            connectend = random.Next(0, connector == 0 ? dimension - 1 : dimension);
        }

        return connectend;
    }

    /// <summary>
    /// Creates a graph based on an adjacency matrix.
    /// </summary>
    /// <param name="adjacencyMatrix">Adjacency matrix representing the graph</param>
    /// <returns>The node represented by the first entry in the adjacency matrix, and the node represented by the last</returns>
    public static (Node start, Node finish) CreateGraph(Matrix adjacencyMatrix)
    {
        //      node id     node         (node id, connection weight) array
        Dictionary<int, (Node node, List<(int id, int weight)> connections)> nodeLegend = new(); // could just be an array?
        for (int i = 0; i < adjacencyMatrix.Rows; i++)
        {
            List<(int, int)> connections = new(adjacencyMatrix.Columns);
            for (int j = 0; j < adjacencyMatrix.Columns; j++)
            {
                if (adjacencyMatrix[i, j] == 0) continue;
                connections.Add((j, (int)adjacencyMatrix[i, j]));
            }
            Node node = new(new());
            nodeLegend.Add(i, (node, connections));
        }

        foreach (var kvp in nodeLegend)
        {
            kvp.Value.node.Arcs = kvp.Value.connections.ToDictionary(c => (INode)nodeLegend[c.id].node, c => c.weight);
        }

        return (nodeLegend[0].node, nodeLegend[nodeLegend.Count - 1].node);
    }

    public DijkstrasProblemGenerator() : this(new Random()) { }
    public DijkstrasProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}