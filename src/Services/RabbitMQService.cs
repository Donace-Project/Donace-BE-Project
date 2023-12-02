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
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _configuration.GetSection("RabbitMQ").GetValue<string>("Host"),
                Port = _configuration.GetSection("RabbitMQ").GetValue<int>("Port"),
                UserName = _configuration.GetSection("RabbitMQ").GetValue<string>("Username"),
                Password = _configuration.GetSection("RabbitMQ").GetValue<string>("Password"),
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetSection("RabbitMQ").GetValue<string>("Host"),
                Port = _configuration.GetSection("RabbitMQ").GetValue<int>("Port"),
                UserName = _configuration.GetSection("RabbitMQ").GetValue<string>("Username"),
                Password = _configuration.GetSection("RabbitMQ").GetValue<string>("Password"),
            };

            
            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "topic_test", type: ExchangeType.Topic);
            channel.QueueDeclare("request-queue", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                Console.WriteLine($"Received Request: {ea.BasicProperties.CorrelationId}");

                var replyMessage = $"This is your reply: {ea.BasicProperties.CorrelationId}";

                var body = Encoding.UTF8.GetBytes(replyMessage);

                channel.BasicPublish("", ea.BasicProperties.ReplyTo, null, body);

                Console.WriteLine(replyMessage);
            };

            channel.BasicConsume(queue: "request-queue", autoAck: true, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken); // Delay để cho BackgroundService chạy liên tục
            }
        }
    }
}
