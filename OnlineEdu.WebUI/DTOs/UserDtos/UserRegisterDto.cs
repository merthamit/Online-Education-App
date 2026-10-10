using System.ComponentModel.DataAnnotations;

namespace OnlineEdu.WebUI.DTOs.UserDtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Adınızı girin.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyadınızı girin.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Bir kullanıcı adı belirleyin.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3 ile 30 karakter arasında olmalıdır.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-posta adresinizi girin.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bir şifre belirleyin.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifrenizi tekrar girin.")]
        [Compare("Password", ErrorMessage = "Şifreler birbiriyle eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}
