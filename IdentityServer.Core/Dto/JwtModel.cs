namespace IdentityServer.Core.Dto; 

public class JwtModel {
    
    public string Token { get; set; }
    public long ExpiresEpoch { get; set; }
    public DateTimeOffset ExpiresDateTimeOffset { get; set; }
}