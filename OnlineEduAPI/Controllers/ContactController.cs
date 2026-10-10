using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.AboutDtos;
using OnlineEdu.DTO.DTOs.ContactDtos;
using OnlineEdu.Entity.Entities;
using MassTransit;
using OnlineEdu.DTO.DTOs.MessageDtos;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
        public class ContactController : ControllerBase
    {
        private readonly IGenericService<Contact> contactServices;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public ContactController(IGenericService<Contact> contactServices, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            this.contactServices = contactServices;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }
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
        public async Task<IActionResult> Create(CreateContactDto createContactDto)
        {
            var newValue = _mapper.Map<Contact>(createContactDto);
            contactServices.TCreate(newValue);
            
            // Send email to admin via RabbitMQ
            await _publishEndpoint.Publish(new SendEmailCommand 
            { 
                To = "admin@onlineedu.com", 
                Subject = "Yeni İletişim Formu Dolduruldu", 
                Body = $"Gönderen: {createContactDto.Email}\nTelefon: {createContactDto.Phone}\nMesaj: Sisteme yeni bir iletişim isteği düştü."
            });
            
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
