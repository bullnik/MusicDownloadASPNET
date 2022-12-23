using RabbitMQ.Client;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace MusicDownloadASPNET.Rabbit
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly List<string> _cdl = new();

        public RabbitMqService() 
        {
            
        }

        public void SendMusicDownloadRequest(string link)
        {
            SendMessage(link);
        }

        public ReadOnlyCollection<string> GetCompletedDownloadsList()
        {
            return _cdl.AsReadOnly();
        }

        private static void SendMessage(string message)
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "MusicDownloadRequests",
                           durable: false,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                           routingKey: "MusicDownloadRequests",
                           basicProperties: null,
                           body: body);
        }
    }
}
