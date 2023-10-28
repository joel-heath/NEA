using System.Runtime.Versioning;
using System.Threading.Tasks.Dataflow;

namespace NEAConsole;
public record struct ConsoleMessage(string? Value, int Left, int Top, ConsoleColor ForegroundColor, ConsoleColor BackgroundColor, BufferBlock<byte>? BB, bool Pure = false, bool ClearRequested = false)
{

}

public static class Console
{
    private static readonly object lockObj = new();
    //private static readonly BufferBlock<ConsoleMessage> bufferBlock = new();

    // utilising these properties rely on trusting that ALL OTHER THREADS are entirely pure
    public static int CursorLeft
    {
        get
        {
            lock (lockObj)
            {
                return System.Console.CursorLeft;
            }
        }
        set
        {
            lock (lockObj)
            {
                System.Console.CursorLeft = value;
            }
        }
    }
    public static int CursorTop
    {
        get
        {
            lock (lockObj)
            {
                return System.Console.CursorTop;
            }
        }
        set
        {
            lock (lockObj)
            {
                System.Console.CursorTop = value;
            }
        }
    }
    public static ConsoleColor ForegroundColor
    {
        get
        {
            lock (lockObj)
            {
                return System.Console.ForegroundColor;
            }
        }
        set
        {
            lock (lockObj)
            {
                System.Console.ForegroundColor = value;
            }
        }
    }
    public static ConsoleColor BackgroundColor
    {
        get
        {
            lock (lockObj)
            {
                return System.Console.BackgroundColor;
            }
        }
        set
        {
            lock (lockObj)
            {
                System.Console.BackgroundColor = value;
            }
        }
    }

    public static int WindowWidth => System.Console.WindowWidth;
    public static int WindowHeight => System.Console.WindowHeight;
    public static bool KeyAvailable => System.Console.KeyAvailable;

    [SupportedOSPlatform("windows")]
    public static bool CursorVisible { get => System.Console.CursorVisible; set => System.Console.CursorVisible = value; }

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
        lock (lockObj)
        {
            CursorLeft = left;
            CursorTop = top;
        }
    }
    public static void Clear()
    {
        lock (lockObj)
        {
            System.Console.Clear();
        }
    }

    public static void WriteLine() => Write(Environment.NewLine);
    public static void WriteLine(string? value) => Write(value + Environment.NewLine);
    public static void Write(string? value)
    {
        lock (lockObj)
        {
            System.Console.Write(value);
        }
    }
    public static void WritePure(string? value, int left, int top)
    {
        lock (lockObj)
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
        lock (lockObj)
        {
            System.Console.Write(value);
        }
    }

    public static void WriteLine(int? value)
    {
        lock (lockObj)
        {
            System.Console.WriteLine(value);
        }
    }

    public static void WriteLine(object? value)
    {
        lock (lockObj)
        {
            System.Console.WriteLine(value);
        }
    }

    public static void Write(double[]? value)
    {
        lock (lockObj)
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