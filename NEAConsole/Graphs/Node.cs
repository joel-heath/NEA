namespace NEAConsole.Graphs;

public class Node : INode
{
    public Dictionary<INode, int> Arcs { get; set; }
    public int? Value { get; set; }
    public Node(Dictionary<INode, int> arcs, int? value) : this(arcs) => Value = value;
    public Node(Dictionary<INode, int> arcs) => Arcs = arcs;
}