using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.BlogDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher")]
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController(IBlogService _blogService, IMapper _mapper, IImageService _imageService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var value = _blogService.TGetBlogsWithCategories();
            return Ok(value);
        }

        [AllowAnonymous]
        [HttpGet("GetBlogCount")]
        public IActionResult Count()
        {
            var value = _blogService.TCount();
            return Ok(value);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _blogService.TGetBlogWithCategories(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteById(int id)
        {
            var entity = _blogService.TGetById(id);
            if (entity == null) return NotFound();
            _blogService.TDelete(id);
            // CDN temizliği: kayıt silinince görseli de Cloudinary'den sil
            await _imageService.DeleteImageAsync(entity.ImageUrl);
            return Ok("Blog silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateBlogDto createBlogDto)
        {
            var newValue = _mapper.Map<Blog>(createBlogDto);
            _blogService.TCreate(newValue);
            return Ok("Yeni Blog oluşturuldu.");
        }

        [HttpPut]

        public async Task<IActionResult> Update(UpdateBlogDto updateBlogDto)
        {
            var newValue = _mapper.Map<Blog>(updateBlogDto);
            var old = _blogService.TGetById(newValue.BlogId);
            var oldImageUrl = old?.ImageUrl;
            _blogService.TUpdate(newValue);
            // Görsel değiştiyse eskisini Cloudinary'den sil
            if (oldImageUrl != newValue.ImageUrl) await _imageService.DeleteImageAsync(oldImageUrl);
            return Ok("Blog güncellendi.");
        }

        [HttpGet("GetBlogByWriterId/{id}")]
        public IActionResult GetBlogByWriterId(int id)
        {
            var values = _blogService.TGetBlogByWriterId(id);
            var mapper = _mapper.Map<List<ResultBlogDto>>(values);
            return Ok(mapper);
        }

        [AllowAnonymous]
        [HttpGet("GetLast4Blogs")]
        public IActionResult GetLast4Blogs()
        {
            var values = _blogService.TGetLast4BlogsWithCategories();
            var mapper = _mapper.Map<List<ResultBlogDto>>(values);
            return Ok(mapper);
        }

        [AllowAnonymous]
        [HttpGet("GetBlogsByCategoryId/{id}")]
        public IActionResult GetBlogsByCategoryId(int id)
        {
            var values = _blogService.TGetBlogsWithCategoriesByCategoryId(id);
            var mapper = _mapper.Map<List<ResultBlogDto>>(values);
            return Ok(mapper);
        }
    }
}
