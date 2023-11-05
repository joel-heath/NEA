namespace NEAConsole;

public class EscapeException : Exception
{
    public EscapeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    public EscapeException(string? message, Exception? innerException) : base(message, innerException) { }
    public EscapeException(string? message) : base(message) { }
    public EscapeException() { }
}