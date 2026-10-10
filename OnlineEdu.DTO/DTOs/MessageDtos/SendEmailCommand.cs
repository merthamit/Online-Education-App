using System;
namespace OnlineEdu.DTO.DTOs.MessageDtos
{
    public class SendEmailCommand
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}