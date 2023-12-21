using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Misc;
using IdentityServer.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Route("usercontent")]
public class UserContentController : ControllerBase {
    private readonly IUserImageRepository _userImageRepo;
    private readonly IWebHostEnvironment _env;
    public UserContentController(IUserImageRepository userImageRepo, IWebHostEnvironment env) {
        _userImageRepo = userImageRepo;
        _env = env;
    }

    [HttpGet("{userId}")]
    public IActionResult GetUserImageUrl(string userId) {
        var path = _userImageRepo.GetImagePathForUser(userId, _env.WebRootPath);
        return Redirect(path);
    }

    [HttpPost("uploadImage")]
    [Authorize]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile image) {
        var userId = GetUserId();

        var res = await _userImageRepo.SaveImageForUserAsync(userId, ImageFileWrapper.From(image), _env.WebRootPath);
        return res.Match<IActionResult>(
            Ok,
            err =>  err switch {
                ImageOperationException => BadRequest(err.Message),
                _ => StatusCode(500)
            }
        );
    }
}

file class ImageFileWrapper : IImageFile {

    private IFormFile _file;
    
    private ImageFileWrapper(IFormFile file) {
        _file = file;
    }

    public static IImageFile From(IFormFile file) {
        return new ImageFileWrapper(file);
    }
    
    public string ContentDisposition => _file.ContentDisposition;
    public string ContentType => _file.ContentType;
    public string FileName => _file.FileName;
    public long Length => _file.Length;
    public string Name => _file.Name;
    public void CopyTo(Stream target) {
        _file.CopyTo(target);
    }

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) {
        return _file.CopyToAsync(target, cancellationToken);
    }

    public Stream OpenReadStream() {
        return _file.OpenReadStream();
    }
}