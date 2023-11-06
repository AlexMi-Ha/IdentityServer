﻿using IdentityServer.Data.Abstractions;
using IdentityServer.Data.Interfaces.Misc;

namespace IdentityServer.Data.Interfaces.Repositories; 

public interface IUserImageRepository {

    string GetImagePathForUser(string userId);

    Task<Result<string>> SaveImageForUserAsync(string userId, IImageFile image);

    Result DeleteImageForUser(string userId);

}