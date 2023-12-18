using Azure;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
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
        public RabbitMQService(ILogger<RabbitMQService> logger,
                               IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "171.245.205.120", Port = 5672, UserName = "admin", Password = "123456789", VirtualHost = "/" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare("resquest-queue", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                Console.WriteLine($"Received Request: {ea.BasicProperties.CorrelationId}");

                var replyMessage = $"This is your reply test: {ea.BasicProperties.CorrelationId}";

                var body = Encoding.UTF8.GetBytes(replyMessage);

                channel.BasicPublish("", ea.BasicProperties.ReplyTo, null, body);
            };

            channel.BasicConsume(queue: "resquest-queue", autoAck: true, consumer: consumer);

            await Task.Delay(-1, stoppingToken);
        }
    }
}
