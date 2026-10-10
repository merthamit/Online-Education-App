using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.UserDtos;
using OnlineEdu.WebUI.Helpers;
using OnlineEdu.WebUI.Services.UserServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _client;

        public RegisterController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(UserRegisterDto userRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userRegisterDto);
            }

            var result = await _client.PostAsJsonAsync("users/register", userRegisterDto);
            if (!result.IsSuccessStatusCode)
            {
                foreach (var error in await ReadErrorsAsync(result))
                {
                    ModelState.AddModelError("", error);
                }
                return View(userRegisterDto);
            }
            return RedirectToAction("SignIn", "Login");
        }

        // API hem Identity hata listesi ([{code, description}]) hem de ValidationProblemDetails ({errors: {...}}) dönebilir
        private static async Task<List<string>> ReadErrorsAsync(HttpResponseMessage response)
        {
            var errors = new List<string>();
            try
            {
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in root.EnumerateArray())
                    {
                        if (item.TryGetProperty("description", out var description))
                            errors.Add(description.GetString());
                    }
                }
                else if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("errors", out var errorObject))
                {
                    foreach (var field in errorObject.EnumerateObject())
                        foreach (var message in field.Value.EnumerateArray())
                            errors.Add(message.GetString());
                }
            }
            catch (JsonException)
            {
            }

            if (errors.Count == 0)
            {
                errors.Add("Kayıt işlemi sırasında bir hata oluştu.");
            }
            return errors;
        }
    }
}
