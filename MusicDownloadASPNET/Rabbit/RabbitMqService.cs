using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace MusicDownloadASPNET.Rabbit
{
    public class RabbitMqService : IRabbitMqService
    {
        private IConnection _connection;
        private IModel _channel;
        private List<string> _cdl = new();
        private string _requestQueueName = "MusicDownloadRequests";
        private string _resultsQueueName = "MusicDownloadResults";

        public RabbitMqService() 
        {
            Console.WriteLine("CREATING CONNECTION");
            var factory = GetConnectionFactory();
            _connection = factory.CreateConnection();
            Console.WriteLine("CONNECTION CREATED");
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _resultsQueueName, durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _cdl.Add(message);
                Console.WriteLine("RECEIVED MESSAGE " + message);
            };
            _channel.BasicConsume(queue: _resultsQueueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void SendMusicDownloadRequest(string link)
        {
            Console.WriteLine("SENDING MESSAGE " + link);
            SendMessage(link);
        }

        public ReadOnlyCollection<string> GetCompletedDownloadsList()
        {
            return _cdl.AsReadOnly();
        }

        private void SendMessage(string message)
        {
            var factory = GetConnectionFactory();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: _requestQueueName,
                           durable: false,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                           routingKey: _requestQueueName,
                           basicProperties: null,
                           body: body);
        }

        private ConnectionFactory GetConnectionFactory()
        {
            return new ConnectionFactory
            {
                UserName = "bykrabbit",
                Password = "bykbykbyk",
                VirtualHost = "/",
                HostName = "rabbit",
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
        }
    }
}
