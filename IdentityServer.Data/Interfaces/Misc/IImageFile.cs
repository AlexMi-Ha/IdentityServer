namespace IdentityServer.Data.Interfaces.Misc; 

public interface IImageFile {
    
    string ContentDisposition { get; }
    string ContentType { get; }
    string FileName { get; }
    long Length { get; }
    string Name { get; }
    
    void CopyTo (Stream target);
    Task CopyToAsync (Stream target, CancellationToken cancellationToken = default);

    Stream OpenReadStream();

}