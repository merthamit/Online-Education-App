using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OnlineEdu.Business.Abstract;
using OnlineEdu.Business.Configurations;
using System.Threading.Tasks;

namespace OnlineEdu.Business.Concrete
{
    public class CloudinaryImageManager : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _cloudName;
        private readonly ILogger<CloudinaryImageManager> _logger;

        public CloudinaryImageManager(IOptions<CloudinarySettings> config, ILogger<CloudinaryImageManager> logger)
        {
            _cloudName = config.Value.CloudName;
            _logger = logger;
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "online_edu"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task DeleteImageAsync(string? imageUrl)
        {
            var publicId = GetPublicId(imageUrl);
            if (publicId == null) return;

            try
            {
                var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
                _logger.LogInformation("[Cloudinary] {PublicId} silindi: {Result}", publicId, result.Result);
            }
            catch (Exception ex)
            {
                // Görsel silinemese bile kaydın silinmesini engellemeyelim
                _logger.LogWarning(ex, "[Cloudinary] {PublicId} silinemedi", publicId);
            }
        }

        // https://res.cloudinary.com/{cloud}/image/upload/v123/online_edu/abc.png -> online_edu/abc
        private string? GetPublicId(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || !Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri)) return null;
            if (!uri.Host.Equals("res.cloudinary.com", StringComparison.OrdinalIgnoreCase)) return null;

            var segments = uri.AbsolutePath.Trim('/').Split('/');
            var uploadIndex = Array.IndexOf(segments, "upload");
            if (segments.Length < 4 || segments[0] != _cloudName || uploadIndex < 0) return null;

            var rest = segments.Skip(uploadIndex + 1).ToList();
            if (rest.Count > 0 && Regex.IsMatch(rest[0], @"^v\d+$")) rest.RemoveAt(0);
            if (rest.Count == 0) return null;

            var path = string.Join("/", rest);
            var dot = path.LastIndexOf('.');
            return dot > 0 ? path[..dot] : path;
        }
    }
}