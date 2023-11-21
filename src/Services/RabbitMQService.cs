using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Donace_BE_Project.Services
{
    public class RabbitMQService : BackgroundService, IRabbitMQService
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly RabbitMQ.Client.IConnection _connection;
        private readonly ConnectionFactory _connectionFactory;
        private readonly Microsoft.EntityFrameworkCore.Metadata.IModel _channel;
        private readonly Dictionary<string, Func<string, string>> _queueHandler;
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
            _channel = (Microsoft.EntityFrameworkCore.Metadata.IModel?)_connection.CreateModel();

            _queueHandler = new Dictionary<string, Func<string, string>>
            {
                {"donace_getdata", ProcessQueue1 }
            };
        }

        private string ProcessQueue1(string message)
        {
            // Xử lý message từ Queue 1 và trả về response
            return $"Processed Queue 1 message: {message}";
        }

        public async Task SubReplyAsync(string message)
        {
            try
            {

            }
            catch(Exception ex)
            {

            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                throw new Exception();
            }
            catch(Exception ex)
            {
                _logger.LogError($"RabbitMQService.ExecuteAsync.Exception: {ex.Message}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_RabbitMQService, ex.Message);
            }
        }
    }
}
