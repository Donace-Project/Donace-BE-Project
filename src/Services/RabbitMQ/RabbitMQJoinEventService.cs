using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using Donace_BE_Project.Models.Eto;
using Donace_BE_Project.Interfaces.Repositories;

namespace Donace_BE_Project.Services.RabbitMQ
{
    public class RabbitMQJoinEventService : BackgroundService
    {
        private readonly IHttpClientService _httpClientService;
        public RabbitMQJoinEventService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "171.245.205.120", Port = 5672, UserName = "admin", Password = "123456789", VirtualHost = "/" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare("request-join-event", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var dataRequest = JsonConvert.DeserializeObject<JoinEventEto>(body);

                await _httpClientService.CallApiPost(@"http://171.245.205.120:8082/", "api/Event/user-join", dataRequest);
                channel.BasicPublish("", ea.BasicProperties.ReplyTo, null, Encoding.UTF8.GetBytes(body));
            };

            channel.BasicConsume(queue: "request-join-event", autoAck: true, consumer: consumer);

            await Task.Delay(-1, stoppingToken);
        }
    }
}
