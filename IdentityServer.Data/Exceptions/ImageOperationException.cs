using System.Runtime.Serialization;

namespace IdentityServer.Data.Exceptions; 

public class ImageOperationException : Exception {
    public ImageOperationException() { }
    protected ImageOperationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public ImageOperationException(string? message) : base(message) { }
    public ImageOperationException(string? message, Exception? innerException) : base(message, innerException) { }
}