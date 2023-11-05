namespace NEAConsole.Matrices;

public class DimensionLessThanOneException : Exception
{
    public DimensionLessThanOneException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    public DimensionLessThanOneException(string? message, Exception? innerException) : base(message, innerException) { }
    public DimensionLessThanOneException(string? message) : base(message) { }
    public DimensionLessThanOneException() { }
}

public class MatrixNotSquareException : Exception
{
    public MatrixNotSquareException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    public MatrixNotSquareException(string? message, Exception? innerException) : base(message, innerException) { }
    public MatrixNotSquareException(string? message) : base(message) { }
    public MatrixNotSquareException() { }
}

public class MatrixSingularException : Exception
{
    public MatrixSingularException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    public MatrixSingularException(string? message, Exception? innerException) : base(message, innerException) { }
    public MatrixSingularException(string? message) : base(message) { }
    public MatrixSingularException() { }
}