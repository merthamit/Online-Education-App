using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OnlineEdu.Business.Abstract
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);

        // Görsel bu hesabın Cloudinary'sindeyse siler; başka bir adresse (ör. Unsplash) hiçbir şey yapmaz
        Task DeleteImageAsync(string? imageUrl);
    }
}