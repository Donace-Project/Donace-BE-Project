using Azure;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.Eto;
using Nest;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Donace_BE_Project.Services
{
    public class RabbitMQService : BackgroundService, IRabbitMQService
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IModel _channel;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientService _httpClientService;
        public RabbitMQService(ILogger<RabbitMQService> logger,
                               IConfiguration configuration,
                               IHttpClientService httpClientService)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientService = httpClientService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "171.245.205.120", Port = 5672, UserName = "admin", Password = "123456789", VirtualHost = "/" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare("request-join-calendar", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var dataRequest = JsonConvert.DeserializeObject<JoinCalendarEto>(body);

                await _httpClientService.CallApiPost(@"http://171.245.205.120:8082/", "api/Calendar/user-join", dataRequest);
                channel.BasicPublish("", ea.BasicProperties.ReplyTo, null, Encoding.UTF8.GetBytes(body));
            };

            channel.BasicConsume(queue: "request-join-calendar", autoAck: true, consumer: consumer);

            await Task.Delay(-1, stoppingToken);
        }
    }
}
