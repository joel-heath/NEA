using System;
using NEAConsole.Matrices;

namespace NEAConsole;
internal class Program
{
    static void ListChoices(string[] options, int chosen)
    {
        for (int i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{(i == chosen ? ">" : " ")} {options[i]} {(i == chosen ? "<" : " ")}");
        }
    }

    static int Choose(string[] options)
    {
        int choice = 0;
        while (true)
        {
            Console.Clear();
            ListChoices(options, choice);
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.UpArrow: choice = choice == 0 ? choice : choice - 1; break;
                case ConsoleKey.DownArrow: choice = choice == options.Length - 1 ? choice : choice + 1; break;
                case ConsoleKey.Enter: return choice;
            }
        }
    }

    static void DrawMatrix(Matrix m) // naive; needs centering cols based on number widths
    {
        int xIndent = Console.CursorLeft;
        int initY = Console.CursorTop;
        for (int i = 0; i < m.Rows; i++)
        {
            Console.Write('[');
            for (int j = 0; j < m.Columns; j++)
            {
                Console.Write($"{m[i, j]}{(j == m.Columns - 1 ? ']' : " ")}");
            }
            if (i < m.Rows - 1)
            {
                Console.CursorLeft = xIndent;
                Console.CursorTop++;
            }
        }

        Console.CursorTop = initY;
    }



    static void DrawInputMatrix(string[][] m, int x, int y)
    {
        int xIndent = Console.CursorLeft;
        int yIndent = Console.CursorTop;

        for (int i = 0; i < m.Length; i++)
        {
            Console.Write('[');
            for (int j = 0; j < m[0].Length; j++)
            {
                Console.BackgroundColor = (i == y && j == x) ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                Console.Write(m[i][j]);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(' ');
            }
            Console.CursorLeft--;
            Console.Write(']');
            Console.CursorLeft = xIndent;
            Console.CursorTop++;
        }

        Console.CursorTop = yIndent;
    }

    static string[][] EmptyInputMat(int rows, int cols) // the pains of a 2d array
    {
        string[][] inputs = new string[rows][];

        for (int i = 0; i < rows; i++)
        {
            inputs[i] = new string[cols];
            for (int j = 0; j < cols; j++)
            {
                inputs[i][j] = " ";
            }
        }

        return inputs;
    }

    static Matrix InputMatrix(int rows, int cols)
    {
        Console.CursorVisible = false;

        Matrix? m = null!; // compilation purposes, wont ever return null
        string[][] inputs = EmptyInputMat(rows, cols); // why is it a jagged array you ask? so i can select through it

        int x = 0, y = 0;
        DrawInputMatrix(inputs, x, y);

        bool entering = true;
        bool redraw = false;
        while (entering)
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.RightArrow:
                    if (x < cols - 1) x++;
                    redraw = true;
                    break;
                case ConsoleKey.LeftArrow:
                    if (x > 0) x--;
                    redraw = true;
                    break;
                case ConsoleKey.UpArrow:
                    if (y < rows - 1) y++;
                    redraw = true;
                    break;
                case ConsoleKey.DownArrow:
                    if (y > 0) y--;
                    redraw = true;
                    break;


                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    if (x < cols - 1) x++;
                    else (x, y) = (0, y + 1);
                    redraw = true;
                    break;

                case ConsoleKey.Escape:
                    entering = false;
                    break;



                case ConsoleKey.Backspace:
                    if (inputs[x][y].Length > 0)
                    {
                        
                    }
                    break;

            }
            if (redraw) DrawInputMatrix(inputs, x, y);

            if (!entering)
            {
                try
                {
                    var doubles = inputs.Select(r => r.Select(e => double.Parse(e)));
                    m = new Matrix(rows, cols, doubles);
                }
                catch (FormatException)
                {
                    entering = true;
                }
            }
        }

        Console.CursorVisible = true;
        return m;
    }

    static void MatricesTest()
    {
        Console.Clear();

        (int rows, int cols) = (Random.Shared.Next(1, 4), Random.Shared.Next(1, 4));
        Matrix mat1 = new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)Random.Shared.Next(-10, 10)));
        Matrix mat2 = new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)Random.Shared.Next(-10, 10)));

        DrawMatrix(mat1);
        
        Console.CursorTop++;
        Console.Write(" + ");
        Console.CursorTop--;
        DrawMatrix(mat2);

        Console.CursorTop++;
        Console.Write(" + ");
        Console.CursorTop--;

        Matrix answer = InputMatrix(rows, cols);
    }

    static void SimplexTest()
    {

    }

    static void MathsMenu()
    {

    }
    static void FMathsMenu()
    {
        string[] options = { "Matrices", "Simplex", "Hypothesis Testing", "Dijkstra's", "Prim's", "Return" };

        Console.WriteLine("Choose a topic to be tested on");

        while (true)
        {
            switch (Choose(options))
            {
                case 0: MatricesTest(); break;
                case 1: SimplexTest(); break;
                case 6: return;
            }
        }
    }
    static void CSciMenu()
    {

    }

    static void Main(string[] args)
    {
        string[] options = { "Maths", "Further Maths", "Computer Science", "Exit" };

        Console.WriteLine("Choose a subject to revise");

        bool @continue = true;
        while (@continue)
        {
            switch (Choose(options))
            {
                case 0: MathsMenu(); break;
                case 1: FMathsMenu(); break;
                case 2: CSciMenu(); break;
                case 3: @continue = false; break;
            }
        }

    }
}