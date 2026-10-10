using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.MessageDtos;
using OnlineEdu.Entity.Entities;
using MassTransit;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
        public class MessagesController : ControllerBase
    {
        private readonly IGenericService<Message> _messageService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public MessagesController(IGenericService<Message> messageService, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _messageService = messageService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var value = _messageService.TGetList();
            return Ok(value);
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _messageService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteById(int id)
        {
            _messageService.TDelete(id);
            return Ok("Message silindi.");
        }
                [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(CreateMessageDto createMessageDto)
        {
            var newValue = _mapper.Map<Message>(createMessageDto);
            _messageService.TCreate(newValue);
            
            // Send email via RabbitMQ
            await _publishEndpoint.Publish(new SendEmailCommand 
            { 
                To = createMessageDto.Email, 
                Subject = "Mesajınız Alındı: " + createMessageDto.Subject, 
                Body = $"Merhaba {createMessageDto.Name},\n\nMesajınız başarıyla bize ulaşmıştır. En kısa sürede size dönüş yapacağız.\n\nİlettiğiniz mesaj: {createMessageDto.Content}"
            });
            
            return Ok("Yeni Message oluşturuldu.");
        }

        [HttpPut]

        public IActionResult Update(UpdateMessageDto updateMessageDto)
        {
            var newValue = _mapper.Map<Message>(updateMessageDto);
            _messageService.TUpdate(newValue);
            return Ok("Message güncellendi.");
        }
    }
}
