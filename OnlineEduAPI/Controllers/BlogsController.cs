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
    public class BlogsController(IBlogService _blogService, IMapper _mapper) : ControllerBase
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

        public IActionResult DeleteById(int id)
        {
            _blogService.TDelete(id);
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

        public IActionResult Update(UpdateBlogDto updateBlogDto)
        {
            var newValue = _mapper.Map<Blog>(updateBlogDto);
            _blogService.TUpdate(newValue);
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
