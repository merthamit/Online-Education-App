using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.AboutDtos;
using OnlineEdu.DTO.DTOs.ContactDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController(IGenericService<Contact> contactServices, IMapper _mapper) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var values = contactServices.TGetList();
            return Ok(values);
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = contactServices.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteById(int id)
        {
            contactServices.TDelete(id);
            return Ok("Contact alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateContactDto createContactDto)
        {
            var newValue = _mapper.Map<Contact>(createContactDto);
            contactServices.TCreate(newValue);
            return Ok("Yeni Contact alanı oluşturuldu.");
        }

        [HttpPut]

        public IActionResult Update(UpdateContactDto updateContactDto)
        {
            var newValue = _mapper.Map<Contact>(updateContactDto);
            contactServices.TUpdate(newValue);
            return Ok("Contact alanı güncellendi.");
        }
    }
}
