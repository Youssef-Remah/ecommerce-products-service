using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BusinessLogic.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly string hostname, username, password, port;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQPublisher(IConfiguration configuration)
        {
            _configuration = configuration;

            hostname = _configuration["RabbitMQ_HostName"]!;
            username = _configuration["RabbitMQ_UserName"]!;
            password = _configuration["RabbitMQ_Password"]!;
            port = _configuration["RabbitMQ_Port"]!;

            var connectionFactory = new ConnectionFactory()
            {
                HostName = hostname,
                UserName = username,
                Password = password,
                Port = Convert.ToInt32(port)
            };
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }

        public void Publish<T>(string routingKey, T message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageInBytes = Encoding.UTF8.GetBytes(messageJson);
            var exchangeName = _configuration["RabbitMQ_Products_Exchange"];

            //Create Exchange
            _channel.ExchangeDeclare(exchange: exchangeName,
                                     type: ExchangeType.Direct,
                                     durable: true);
            //Publish Message
            _channel.BasicPublish(exchange: exchangeName,
                                  routingKey: routingKey,
                                  basicProperties: null,
                                  body: messageInBytes);
        }
    }
}
