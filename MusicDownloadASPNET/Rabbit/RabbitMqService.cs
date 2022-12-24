using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MusicDownloadASPNET.Rabbit
{
    public class RabbitMqService : IRabbitMqService
    {
        private string _requestQueueName = "MusicDownloadRequests";

        public RabbitMqService() 
        {

        }

        public void SendMusicDownloadRequest(string link)
        {
            Console.WriteLine("SENDING MESSAGE " + link);
            SendMessage(link);
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
