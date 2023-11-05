using System.Runtime.Serialization;

namespace IdentityServer.Data.Exceptions; 

public class AuthFailureException : Exception {
    public AuthFailureException() { }
    protected AuthFailureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public AuthFailureException(string? message) : base(message) { }
    public AuthFailureException(string? message, Exception? innerException) : base(message, innerException) { }
}