using System.Text.RegularExpressions;
using IdentityServer.Core.Interfaces.Misc;

namespace IdentityServer.Data.Extensions; 

public static class IImageFileExtensions {
    
    public const int ImageMinimumBytes = 512;

    public static bool IsImage(this IImageFile postedFile) {
        //-------------------------------------------
        //  Check the image mime types
        //-------------------------------------------
        if (!postedFile.ContentType.Equals("image/jpg", StringComparison.CurrentCultureIgnoreCase) &&
            !postedFile.ContentType.Equals("image/jpeg", StringComparison.CurrentCultureIgnoreCase) &&
            //postedFile.ContentType.ToLower() != "image/pjpeg" &&
            //postedFile.ContentType.ToLower() != "image/gif" &&
            //postedFile.ContentType.ToLower() != "image/x-png" &&
            //postedFile.ContentType.ToLower() != "image/pjpeg" &&
            //postedFile.ContentType.ToLower() != "image/gif" &&
            //postedFile.ContentType.ToLower() != "image/x-png" &&
            !postedFile.ContentType.Equals("image/png", StringComparison.CurrentCultureIgnoreCase)) {
            return false;
        }

        //-------------------------------------------
        //  Check the image extension
        //-------------------------------------------
        if (!Path.GetExtension(postedFile.FileName).Equals(".jpg", StringComparison.CurrentCultureIgnoreCase)
            && !Path.GetExtension(postedFile.FileName).Equals(".png", StringComparison.CurrentCultureIgnoreCase)
            //&& Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
            && !Path.GetExtension(postedFile.FileName).Equals(".jpeg", StringComparison.CurrentCultureIgnoreCase)) {
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