using System.ComponentModel.DataAnnotations;

namespace OnlineEdu.WebUI.DTOs.UserDtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "E-posta adresinizi girin.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifrenizi girin.")]
        public string Password { get; set; }
    }
}
