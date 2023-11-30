using System.Runtime.InteropServices;

namespace NEAConsole;

public static class Console
{
    public static readonly object Lock = new();

    // utilising these properties rely on trusting that ALL OTHER THREADS are entirely pure
    public static int CursorLeft
    {
        get
        {
            lock (Lock)
            {
                return System.Console.CursorLeft;
            }
        }
        set
        {
            lock (Lock)
            {
                System.Console.CursorLeft = value;
            }
        }
    }
    public static int CursorTop
    {
        get
        {
            lock (Lock)
            {
                return System.Console.CursorTop;
            }
        }
        set
        {
            lock (Lock)
            {
                System.Console.CursorTop = value;
            }
        }
    }
    public static ConsoleColor ForegroundColor
    {
        get
        {
            lock (Lock)
            {
                return System.Console.ForegroundColor;
            }
        }
        set
        {
            lock (Lock)
            {
                System.Console.ForegroundColor = value;
            }
        }
    }
    public static ConsoleColor BackgroundColor
    {
        get
        {
            lock (Lock)
            {
                return System.Console.BackgroundColor;
            }
        }
        set
        {
            lock (Lock)
            {
                System.Console.BackgroundColor = value;
            }
        }
    }

    public static int WindowWidth => System.Console.WindowWidth;
    public static int WindowHeight => System.Console.WindowHeight;
    public static bool KeyAvailable => System.Console.KeyAvailable;

    public static bool CursorVisible
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return System.Console.CursorVisible;
            return false;
        }
        set
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                System.Console.CursorVisible = value;
        }
    }

    public static ConsoleKeyInfo ReadKey(bool intercept) => System.Console.ReadKey(intercept);

    public static void ClearKeyBuffer()
    {
        while (System.Console.KeyAvailable)
        {
            System.Console.ReadKey(true);
        }
    }

    public static void SetCursorPosition(int left, int top)
    {
        lock (Lock)
        {
            CursorLeft = left;
            CursorTop = top;
        }
    }
    public static void Clear()
    {
        lock (Lock)
        {
            System.Console.Clear();
        }
    }

    public static void WriteLine() => Write(Environment.NewLine);
    public static void WriteLine(string? value) => Write(value + Environment.NewLine);
    public static void Write(string? value)
    {
        lock (Lock)
        {
            System.Console.Write(value);
        }
    }
    public static void WritePure(string? value, int left, int top)
    {
        lock (Lock)
        {
            var (oldLeft, oldTop) = System.Console.GetCursorPosition();
            System.Console.SetCursorPosition(left, top);
            System.Console.Write(value);
            System.Console.SetCursorPosition(oldLeft, oldTop);
        }
    }

    // NOTHING NEW BELOW: just all boring functions to accept all inputs
    public static void Write(char? value)
    {
        lock (Lock)
        {
            System.Console.Write(value);
        }
    }

    public static void WriteLine(int? value)
    {
        lock (Lock)
        {
            System.Console.WriteLine(value);
        }
    }

    public static void WriteLine(object? value)
    {
        lock (Lock)
        {
            System.Console.WriteLine(value);
        }
    }

    public static void Write(double[]? value)
    {
        lock (Lock)
        {
            System.Console.Write(value);
        }
    }

    /*public static void Write(object? value)
    {
        lock (lockObj)
        {
            System.Console.Write(value);
        }
    }*/
}