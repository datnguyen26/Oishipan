using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace OishipanAPI.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName = "oishipan")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = folderName,
                        Transformation = new Transformation()
                            .Quality("auto")
                            .FetchFormat("auto")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                        return null;

                    return uploadResult.SecureUrl.ToString();
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                    return false;

                var deleteParams = new DeletionParams(publicId);
                var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

                return deleteResult.Result == "ok";
            }
            catch
            {
                return false;
            }
        }
    }
}
