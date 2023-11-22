namespace NEAConsole;

public class EscapeException : Exception
{
    public EscapeException(string? message, Exception? innerException) : base(message, innerException) { }
    public EscapeException(string? message) : base(message) { }
    public EscapeException() : base() { }
}