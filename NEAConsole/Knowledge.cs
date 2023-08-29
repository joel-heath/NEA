namespace NEAConsole;
public record Knowledge(Skill Matrices, Skill Graphs, Skill Simplex)
{
    public Skill[] AsArray => new[] { Matrices, Graphs, Simplex };
}
public class Skill
{
    public string Name { get; }
    //public string Description { get; }
    //public int Weight { get; }
    //public DateTime LastRevised { get; }
    public IProblemGenerator? ProblemGenerator { get; }
    public List<Skill> Children { get; }
    public bool Known { get; set; }

    public Skill(string name) : this(name, new List<Skill>()) { }
    public Skill(string name, List<Skill> children, IProblemGenerator? problemGenerator = null, bool known = false)
    {
        Name = name;
        Known = known;
        Children = children;
        ProblemGenerator = problemGenerator;
    }
}