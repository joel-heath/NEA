using NEAConsole.Matrices;

namespace NEAConsole;

public static class UIMethods
{
    /// <summary>
    /// Draws a matrix to the console
    /// </summary>
    /// <param name="m">Matrix to be drawn</param>
    /// <param name="resetY">Defaults to true, if true Console.CursorTop will be reset to the top of the matrix, otherwise it'll be at the bottom of the matrix.</param>
    public static void DrawMatrix(Matrix m, bool resetY = true)
    {
        var widths = GetMatrixWidths(m);

        int xIndent = Console.CursorLeft;
        int initY = Console.CursorTop;
        for (int i = 0; i < m.Rows; i++)
        {
            Console.Write('[');
            for (int j = 0; j < m.Columns; j++)
            {
                var num = m[i, j];
                var len = num.ToString().Length;
                var spaces = (widths[j] - len) / 2;
                Console.Write($"{new string(' ', spaces)}{num}{new string(' ', widths[j] - spaces - len)}{(j < m.Columns - 1 ? " " : "]")}");
            }
            if (i < m.Rows - 1)
            {
                Console.CursorLeft = xIndent;
                Console.CursorTop++;
            }
        }

        if (resetY) Console.CursorTop = initY;
    }

    /// <summary>
    /// Draw matrix with row and column titles A, B, C, etc.
    /// </summary>
    /// <param name="m">Matrix to be drawn</param>
    public static void DrawTitledMatrix(Matrix m)
    {
        var widths = GetMatrixWidths(m);

        int xIndent = Console.CursorLeft;
        // Column titles
        Console.Write("   ");
        for (int i = 0; i < m.Columns; i++)
        {
            var name = (char)('A' + i);
            var spaces = (widths[i] - 1) / 2;
            Console.Write($"{new string(' ', spaces)}{name}{new string(' ', widths[i] - spaces)}");
        }
        Console.WriteLine();
        for (int i = 0; i < m.Rows; i++)
        {
            //                  Row title
            Console.Write($"{(char)('A' + i)} [");
            for (int j = 0; j < m.Columns; j++)
            {
                string val = m[i, j] == 0 ? "-" : m[i, j].ToString();
                var len = val.Length;
                var spaces = (widths[j] - len) / 2;
                Console.Write($"{new string(' ', spaces)}{val}{new string(' ', widths[j] - spaces - len)}{(j < m.Columns - 1 ? " " : "]")}");
            }
            if (i < m.Rows - 1)
            {
                Console.CursorLeft = xIndent;
                Console.CursorTop++;
            }
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Writes each element in the matrix in CSV form, commas separating columns and new lines separating rows. Exclusively for debugging.
    /// </summary>
    /// <param name="m">Matrix to be drawn.</param>
    public static void DebugDrawMatrix(Matrix m)
    {
        for (int i = 0; i < m.Rows; i++)
        {
            for (int j = 0; j < m.Columns; j++)
            {
                Console.Write($"{m[i, j]},");
            }
            Console.CursorLeft--;
            Console.WriteLine(' ');
        }
    }

    public static int[] GetMatrixWidths(Matrix m)
    {
        int[] widths = new int[m.Columns];
        for (int c = 0; c < m.Columns; c++)
        {
            int max = 0;
            for (int r = 0; r < m.Rows; r++)
            {
                var len = m[r, c].ToString().Length;
                if (len > max) max = len;
            }

            widths[c] = max;
        }

        return widths;
    }

    public static void UpdateAllSkills(IEnumerable<Skill> skills, IEnumerable<string>? skillPath = null)
    {
        skillPath ??= new List<string>();
        foreach (Skill skill in skills)
        {
            var newPath = skillPath.Append(skill.Name);
            Console.WriteLine($"Do you know {string.Join(" > ", newPath)}?");

            bool response = Menu.Affirm();
            Console.Clear();

            if (response) skill.Known = true;
            else continue;

            if (skill.Children.Length > 0)
            {
                UpdateAllSkills(skill.Children, newPath);
            }
        }
    }

    public static void UpdateKnownSkills(IEnumerable<Skill> skills, IEnumerable<string>? skillPath = null)
    {
        skillPath ??= Array.Empty<string>();
        foreach (Skill skill in skills.Where(s => s.Known))
        {
            var newPath = skillPath.Append(skill.Name);
            Console.WriteLine($"Do you want to be tested on {string.Join(" > ", newPath)}?");

            bool response = Menu.Affirm();
            Console.Clear();

            if (!response) skill.Known = false;

            if (skill.Children.Length > 0)
            {
                UpdateKnownSkills(skill.Children, skillPath); // or new path if we want Matrices > Determinants > Inverses, but we don't
            }
        }
    }
}