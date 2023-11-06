using System.Text.RegularExpressions;
using IdentityServer.Data.Interfaces.Misc;

namespace IdentityServer.Data.Extensions; 

public static class IImageFileExtensions {
    
    public const int ImageMinimumBytes = 512;

    public static bool IsImage(this IImageFile postedFile) {
        //-------------------------------------------
        //  Check the image mime types
        //-------------------------------------------
        if (postedFile.ContentType.ToLower() != "image/jpg" &&
            postedFile.ContentType.ToLower() != "image/jpeg" &&
            postedFile.ContentType.ToLower() != "image/pjpeg" &&
            postedFile.ContentType.ToLower() != "image/gif" &&
            postedFile.ContentType.ToLower() != "image/x-png" &&
            postedFile.ContentType.ToLower() != "image/png") {
            return false;
        }

        //-------------------------------------------
        //  Check the image extension
        //-------------------------------------------
        if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg") {
            return false;
        }

        //-------------------------------------------
        //  Attempt to read the file and check the first bytes
        //-------------------------------------------
        try {
            using (var stream = postedFile.OpenReadStream()) {
                try {

                    if (!stream.CanRead) {
                        return false;
                    }

                    //------------------------------------------
                    //check whether the image size exceeding the limit or not
                    //------------------------------------------ 
                    if (postedFile.Length < ImageMinimumBytes) {
                        return false;
                    }

                    byte[] buffer = new byte[ImageMinimumBytes];
                    stream.Read(buffer, 0, ImageMinimumBytes);
                    string content = System.Text.Encoding.UTF8.GetString(buffer);
                    if (Regex.IsMatch(content,
                            @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline)) {
                        return false;
                    }
                }
                catch (Exception) {
                    return false;
                }
                finally {
                    stream.Position = 0;
                }
            }
        }
        catch (Exception) {
            return false;
        }
        return true;

    }
}