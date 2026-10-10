using Microsoft.AspNetCore.Identity;

namespace OnlineEdu.Business.Validators
{
    // Identity hata mesajlarının Türkçe karşılıkları
    public class CustomErrorDescribe : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresDigit()
            => new() { Code = nameof(PasswordRequiresDigit), Description = "Şifre en az bir rakam içermelidir." };

        public override IdentityError PasswordRequiresLower()
            => new() { Code = nameof(PasswordRequiresLower), Description = "Şifre en az bir küçük harf içermelidir." };

        public override IdentityError PasswordRequiresUpper()
            => new() { Code = nameof(PasswordRequiresUpper), Description = "Şifre en az bir büyük harf içermelidir." };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Şifre en az bir özel karakter (ör. ! * ? @) içermelidir." };

        public override IdentityError PasswordTooShort(int length)
            => new() { Code = nameof(PasswordTooShort), Description = $"Şifre en az {length} karakter olmalıdır." };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"Şifre en az {uniqueChars} farklı karakter içermelidir." };

        public override IdentityError PasswordMismatch()
            => new() { Code = nameof(PasswordMismatch), Description = "Mevcut şifre hatalı." };

        public override IdentityError DuplicateUserName(string userName)
            => new() { Code = nameof(DuplicateUserName), Description = $"\"{userName}\" kullanıcı adı zaten kullanılıyor." };

        public override IdentityError DuplicateEmail(string email)
            => new() { Code = nameof(DuplicateEmail), Description = $"\"{email}\" e-posta adresiyle zaten bir hesap var." };

        public override IdentityError InvalidUserName(string? userName)
            => new() { Code = nameof(InvalidUserName), Description = "Kullanıcı adı yalnızca harf, rakam ve - . _ karakterlerini içerebilir (boşluk kullanılamaz)." };

        public override IdentityError InvalidEmail(string? email)
            => new() { Code = nameof(InvalidEmail), Description = "Geçerli bir e-posta adresi girin." };

        public override IdentityError DefaultError()
            => new() { Code = nameof(DefaultError), Description = "Beklenmeyen bir hata oluştu, lütfen tekrar deneyin." };
    }
}
