namespace NEAConsole;
using System.Text.Json;

public class Skill
{
    public string Name { get; }
    public bool Known { get; set; }
    public int Weight { get; } // settable by user?
    public DateTime LastRevised { get; set; }
    public Skill[] Children { get; set; }

    public IEnumerable<Skill> Traverse(bool respectKnown = true)
    {
        foreach (var child in Children)
        {
            if (!respectKnown || child.Known)
            {
                foreach (var grandchild in child.Traverse())
                {
                    if (!respectKnown || grandchild.Known)
                        yield return grandchild;
                }
                yield return child;
            }
        }
    }

    public bool Query(string skillPath, out Skill? skill)
    {
        if ((skillPath == string.Empty || skillPath == Name) && Known)
        {
            skill = this;
            return true;
        }

        var childName = string.Concat(skillPath.TakeWhile(c => c != '.'));

        try
        {
            var child = Children.First(c => c.Name == childName && c.Known);
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

    public void ResetKnowledge(string treeJSONPath)
    {
        Children = JsonSerializer.Deserialize<Skill[]>(File.ReadAllText(treeJSONPath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    public static Skill KnowledgeConstructor(string treeJSONPath)
        => KnowledgeConstructor(JsonSerializer.Deserialize<Skill[]>(File.ReadAllText(treeJSONPath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!);
    public static Skill KnowledgeConstructor(Skill[] skills)
        => new(string.Empty, true, 0, DateTime.MinValue, skills);

    //[JsonConstructor]
    public Skill(string name, bool known, int weight, DateTime lastRevised, Skill[]? children)
        => (Name, Known, Weight, LastRevised, Children) = (name, known, weight, lastRevised, children ?? Array.Empty<Skill>());
}