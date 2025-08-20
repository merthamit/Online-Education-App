using FluentValidation;
using OnlineEdu.WebUI.DTOs.BlogCategoryDtos;
namespace OnlineEdu.WebUI.Validators
{
public class BlogCategoryValidator : AbstractValidator<CreateBlogCategoryDto>
    {

        public BlogCategoryValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Blog Kategori adı boş bırakılamaz.");
            RuleFor(x => x.Name).MaximumLength(30).WithMessage("Blog Kategori adı boş MAX 30 karakter.");
        }
    }
}
