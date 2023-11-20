namespace IdentityServer.Core.Dto; 

public class ChangeNameModel(string userId, string newName) {

    public string UserId { get; set; } = userId;
    public string NewName { get; set; } = newName;
}