using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace MusicDownloadASPNET.Rabbit
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IDictionary<string, ConcurrentQueue<string>> _queues;
        private readonly IModel _model;

        public RabbitMqService()
        {
            string? rabbitUser = Environment.GetEnvironmentVariable("RABBIT_USER");
            string? rabbitPassword = Environment.GetEnvironmentVariable("RABBIT_PASSWORD");
            string? rabbitHostname = Environment.GetEnvironmentVariable("RABBIT_HOST");

            if (rabbitUser is null || rabbitPassword is null || rabbitHostname is null)
            {
                throw new Exception("Environment not specified");
            }

            _queues = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            _model = GetConnectionFactory(rabbitUser, rabbitPassword, rabbitHostname)
                .CreateConnection()
                .CreateModel();
        }

        public bool SendMessage(string queueName, string message)
        {
            Console.WriteLine("sended");
            DeclareQueue(queueName);
            byte[] body = Encoding.UTF8.GetBytes(message);
            _model.BasicPublish(exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body);
            return true;
        }

        public bool TryReceiveMessage(string queueName, out string message)
        {
            if (!_queues.ContainsKey(queueName))
            {
                SetConsumer(queueName);
                Thread.Sleep(250);
            }

            ConcurrentQueue<string> queue = _queues[queueName];

            bool success = queue.TryDequeue(out string? result);
            message = result is null ? "" : result;
            return success;
        }

        private void SetConsumer(string queueName)
        {
            DeclareQueue(queueName);
            _queues.Add(queueName, new());
            EventingBasicConsumer consumer = new(_model);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                ConcurrentQueue<string> queue = _queues[queueName];
                queue.Enqueue(message);
                Console.WriteLine("received");
            };
            _model.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void DeclareQueue(string queueName)
        {
            _model.QueueDeclare(queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        private static ConnectionFactory GetConnectionFactory(string user, string password, string host)
        {
            return new ConnectionFactory
            {
                UserName = user,
                Password = password,
                VirtualHost = "/",
                HostName = host,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
        }
    }
}
