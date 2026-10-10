using MassTransit;
using OnlineEdu.DTO.DTOs.MessageDtos;
using System.Threading.Tasks;
using System;

namespace OnlineEduAPI.Consumers
{
    public class SendEmailConsumer : IConsumer<SendEmailCommand>
    {
        public async Task Consume(ConsumeContext<SendEmailCommand> context)
        {
            var message = context.Message;
            Console.WriteLine($"[RabbitMQ] E-Posta gönderiliyor: {message.To} | Konu: {message.Subject}");
            // Gerçek projede burada SMTP (MailKit) kullanılır
            await Task.Delay(500); 
            Console.WriteLine($"[RabbitMQ] E-Posta başarıyla gönderildi: {message.To}");
        }
    }
}