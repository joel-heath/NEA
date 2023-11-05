namespace NEAConsole;

public record struct MenuOption(string DisplayText, Action<Context> OnSelected)
{
    public readonly int Length => DisplayText.Length;
    public override readonly string ToString() => DisplayText;
    public static implicit operator MenuOption((string DisplayText, Action<Context> OnSelected) tuple) => new(tuple.DisplayText, tuple.OnSelected);
}

internal static class Menu
{
    static void ListChoices(IList<MenuOption> options)
    {
        var yPos = Console.CursorTop;
        Console.WriteLine($"> {options[0]} <");
        for (int i = 1; i < options.Count; i++)
        {
            Console.WriteLine($"  {options[i]}  ");
        }

        Console.CursorTop = yPos;
    }

    /// <summary>
    /// Allows the user to choose between "Yes" or "No".
    /// </summary>
    /// <param name="ct">Used by Choose() for early exiting during multithreading.</param>
    /// <returns>True if the user chose yes, false if the user chose no.</returns>
    public static bool Affirm(CancellationToken? ct = null)
        => Choose(new MenuOption[] { ("Yes", null!), ("No", null!) }, ct) == 0;

    public static void ExecuteMenu(IEnumerable<IProblemGenerator> options, string prompt, Context context)
        => ExecuteMenu(options.Select(opt => opt.ToMenuOption()), prompt, context);

    /// <summary>
    /// Displays a menu of MenuOptions, allowing the user to continually pick MenuOptions and running their OnSelected, until the user chooses the "Exit" option.
    /// </summary>
    /// <param name="options">IList of MenuOptions representing all the options the user can pick from, "Exit" or "Return" need not be included.</param>
    /// <param name="prompt">The prompt to show the user preceding each option e.g. "Choose a subject to revise" or "Main Menu".</param>
    public static void ExecuteMenu(IEnumerable<MenuOption> options, string prompt, Context context)
    {
        var choices = options.Append(("Exit", null!)).ToList();

        bool @continue = true;
        while (@continue)
        {
            Console.WriteLine(prompt);
            int choice;
            try { choice = Choose(choices); }
            catch (EscapeException) { choice = choices.Count - 1; }

            if (choice == choices.Count - 1)
            {
                @continue = false;
            }
            else
            {
                Console.Clear();
                try
                {
                    choices[choice].OnSelected(context);
                }
                catch (EscapeException)
                {
                    Console.Clear();
                    //@continue = false; this would exit out the current menu--we want to stay in this menu
                }
            }
        }
    }

    /// <summary>
    /// Allows the user to choose from a set of options, returning the index of the chosen options. (Hence OnSelected() is not ran, no restrictions are put on chosen options.)
    /// </summary>
    /// <param name="options">An IList of MenuOptions containing all possible options for the user to choose from.</param>
    /// <param name="ct">Used for early exiting by UIMethods.ReadKey().</param>
    /// <returns>Index of selected option.</returns>
    public static int Choose(IList<MenuOption> options, CancellationToken? ct = null)
    {
        int initX = Console.CursorLeft, initY = Console.CursorTop;
        Console.CursorVisible = false;
        ListChoices(options);

        int choice = 0;
        bool choosing = true;
        while (choosing)
        {
            var changed = false;
            var keyInfo = InputMethods.ReadKey(true, ct);
            var key = keyInfo.Key;
            if (key == ConsoleKey.Tab)
            {
                key = keyInfo.Modifiers == ConsoleModifiers.Shift ? ConsoleKey.UpArrow : ConsoleKey.DownArrow;
            }
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (choice > 0)
                    {
                        Console.CursorLeft = 0;
                        Console.Write(' ');
                        Console.CursorLeft = options[choice].Length + 3;
                        Console.Write(' ');
                        Console.CursorTop--;
                        choice--;
                        changed = true;
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (choice < options.Count - 1)
                    {
                        Console.CursorLeft = 0;
                        Console.Write(' ');
                        Console.CursorLeft = options[choice].Length + 3;
                        Console.Write(' ');
                        Console.CursorTop++;
                        choice++;
                        changed = true;
                    }
                    break;

                case ConsoleKey.Enter:
                    choosing = false;
                    break;

                case ConsoleKey.Escape:
                    throw new EscapeException();
            }

            if (changed)
            {
                Console.CursorLeft = 0;
                Console.Write('>');
                Console.CursorLeft = options[choice].Length + 3;
                Console.Write('<');
            }
        }

        Console.SetCursorPosition(initX, initY);
        Console.CursorVisible = true;
        return choice;
    }

    public static int ExamMenu(string[] options, int question, CancellationToken? ct = null)
    {
        //var (initX, initY) = (Console.CursorLeft, Console.CursorTop);
        Console.CursorTop = 1;
        Console.CursorVisible = false;

        Console.CursorLeft = options[0].Length + 1;
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(options[1]);
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Gray;

        bool choosing = true;
        int choice = 0; // -1 is go back a question (left), 0 is stay on this question (middle), 1 is next question (right)
        while (choosing)
        {
            switch (InputMethods.ReadKey(true, ct).Key)
            {
                case ConsoleKey.LeftArrow:
                    if (choice > (question == 1 ? 0 : -1))
                    {
                        Console.CursorLeft = options.Take(choice + 1).Sum(o => o.Length) + choice + 1; // overwrite old selected
                        Console.Write(options[choice + 1]);
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        choice--;
                        Console.CursorLeft = options.Take(choice + 1).Sum(o => o.Length) + choice + 1; // write new selected
                        Console.Write(options[choice + 1]);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (choice < 1)
                    {
                        Console.CursorLeft = options.Take(choice + 1).Sum(o => o.Length) + choice + 1; // overwrite old selected
                        Console.Write(options[choice + 1]);
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        choice++;
                        Console.CursorLeft = options.Take(choice + 1).Sum(o => o.Length) + choice + 1; // write new selected
                        Console.Write(options[choice + 1]);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    break;
                case ConsoleKey.Enter:
                    choosing = false;
                    break;
                case ConsoleKey.Escape:
                    //or return; need an Are you sure?
                    throw new EscapeException();
            }
        }

        //Console.SetCursorPosition(initX, initY);
        Console.CursorVisible = true;
        return choice;
    }
}