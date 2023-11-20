using System.Runtime.Serialization;
using System.Text;
using FluentValidation.Results;

namespace IdentityServer.Core.Exceptions; 

public class ValidationException : Exception {
    public ValidationException() { }
    protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public ValidationException(string? message) : base(message) { }
    public ValidationException(string? message, Exception? innerException) : base(message, innerException) { }

    public ValidationException(List<ValidationFailure> validationFailures) : base(GetMessage(validationFailures)) {
        
    }

    private static string GetMessage(List<ValidationFailure> fails) {
        var builder = new StringBuilder();
        foreach (var fail in fails) {
            builder.Append(fail.PropertyName);
            builder.Append(": ");
            builder.Append(fail.ErrorMessage);
            builder.Append(";\n");
        }

        return builder.ToString();
    }
}