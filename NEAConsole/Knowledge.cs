using System.Reflection.Metadata.Ecma335;

namespace NEAConsole;
public class Knowledge
{
    public bool Entered { get; set; }
    public Skill Matrices { get; }
    public Skill Graphs { get; }
    public Skill Simplex { get; }
    public Skill[] AsArray => new[] { Matrices, Graphs, Simplex };

    public bool IsKnown(string skillPath) => AsArray.Any(s => s.Query(skillPath[(skillPath.IndexOf('.')+1)..], out Skill? skill) && skill!.Known);

    public Knowledge()
    {
        Entered = false;
    }
    public Knowledge(Skill matrices, Skill graphs, Skill simplex)
    {
        Matrices = matrices;
        Graphs = graphs;
        Simplex = simplex;
        Entered = true;
    }
}
public class Skill
{
    public string Name { get; }
    public bool Known { get; set; }
    public int Weight { get; } // settable by user?
    public DateTime LastRevised { get; set; }
    public Skill[] Children { get; }

    public bool Query(string skillPath, out Skill? skill)
    {
        if (skillPath == string.Empty || skillPath == Name)
        {
            skill = this;
            return true;
        }

        var childName = string.Concat(skillPath.TakeWhile(c => c != '.'));

        try
        {
            var child = Children.First(c => c.Name == childName);
            if (skillPath == childName)
            {
                skill = child;
                return true;
            }
            else
            {
                return child.Query(skillPath[skillPath.IndexOf('.')..][1..], out skill);
            }
        }
        catch (InvalidOperationException)
        {
            skill = null;
            return false;
        }
    }

    //[JsonConstructor]
    public Skill(string name, bool known, int weight, DateTime lastRevised, Skill[] children)
        => (Name, Known, Weight, LastRevised, Children) = (name, known, weight, lastRevised, children);
}