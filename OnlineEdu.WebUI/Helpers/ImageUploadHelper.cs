using System.Net.Http.Headers;

namespace OnlineEdu.WebUI.Helpers
{
    public static class ImageUploadHelper
    {
        private class ImageUploadResponse { public string? Url { get; set; } }

        // Dosyayı API üzerinden Cloudinary CDN'e yükler; dosya yoksa veya yükleme başarısızsa null döner
        public static async Task<string?> UploadImageAsync(this HttpClient client, IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync("ImageUpload", content);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<ImageUploadResponse>();
            return result?.Url;
        }
    }
}
