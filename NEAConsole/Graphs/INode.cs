namespace NEAConsole.Graphs;

public interface INode
{
    public Dictionary<INode, int> Arcs { get; set; }
    public int? Value { get; set; }
}
