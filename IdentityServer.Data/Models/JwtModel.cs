namespace IdentityServer.Data.Models; 

public class JwtModel {
    
    public string Token { get; set; }
    public long Expires { get; set; }
}