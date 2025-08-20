using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.AboutDtos;
using OnlineEdu.DTO.DTOs.BannerDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController(IGenericService<Banner> _bannerService, IMapper _mapper) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]

        public IActionResult Get()
        {
            var value = _bannerService.TGetList();
            return Ok(value);
        }


        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _bannerService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteById(int id)
        {
            _bannerService.TDelete(id);
            return Ok("Banner alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateBannerDto createBannerDto)
        {
            var newValue = _mapper.Map<Banner>(createBannerDto);
            _bannerService.TCreate(newValue);
            return Ok("Yeni Banner alanı oluşturuldu.");
        }

        [HttpPut]

        public IActionResult Update(UpdateBannerDto updateBannerDto)
        {
            var newValue = _mapper.Map<Banner>(updateBannerDto);
            _bannerService.TUpdate(newValue);
            return Ok("Banner alanı güncellendi.");
        }
    }
}
