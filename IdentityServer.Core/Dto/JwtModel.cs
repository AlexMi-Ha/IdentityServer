namespace IdentityServer.Core.Dto; 

public class JwtModel {
    
    public string Token { get; set; }
    public long Expires { get; set; }
}