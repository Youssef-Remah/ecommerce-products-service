using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

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
            throw new NotImplementedException();
        }
    }
}
