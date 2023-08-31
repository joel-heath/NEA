
namespace NEAConsole;
public static class UIMethods
{
    public static int ReadInt()
    {
        bool entering = true;
        string rawNum = string.Empty;
        int pos = 0;
        int indent = Console.CursorLeft;
        while (entering)
        {
            Console.CursorLeft = indent + pos;
            var k = Console.ReadKey(true);
            if (k.KeyChar >= '0' && k.KeyChar <= '9')
            {
                Console.Write(k.KeyChar);
                Console.Write(rawNum[pos..]);
                rawNum = rawNum[..pos] + k.KeyChar + rawNum[pos..];
                pos++;
                continue;
            }
            switch (k.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (pos > 0) pos--;
                    break;
                case ConsoleKey.RightArrow:
                    if (pos < rawNum.Length) pos++;
                    break;
                case ConsoleKey.Home:
                    pos = 0;
                    break;
                case ConsoleKey.End:
                    pos = rawNum.Length;
                    break;
                case ConsoleKey.Delete:
                    if (pos < rawNum.Length)
                    {
                        Console.Write(rawNum[(pos + 1)..] + ' ');
                        rawNum = rawNum[..pos] + rawNum[(pos + 1)..];
                    }
                    break;
                case ConsoleKey.Backspace:
                    if (pos > 0)
                    {
                        Console.CursorLeft--;
                        Console.Write(rawNum[pos--..] + ' ');
                        rawNum = rawNum[..pos] + rawNum[(pos + 1)..];
                    }
                    break;
                //case ConsoleKey.Escape:
                //throw new EscapeException();
                case ConsoleKey.Enter:
                    entering = false;
                    break;
            }
        }

        return int.Parse(rawNum);
    }
}