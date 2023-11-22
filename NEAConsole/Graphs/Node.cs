namespace NEAConsole.Graphs;

public class Node(Dictionary<INode, int> arcs) : INode
{
    public Dictionary<INode, int> Arcs { get; set; } = arcs;
    public int? Value { get; set; }
    public Node(Dictionary<INode, int> arcs, int? value) : this(arcs) => Value = value;
}