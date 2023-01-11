using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MusicDownloadASPNET.Rabbit
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IDictionary<string, Queue<string>> _queues;
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

            _queues = new Dictionary<string, Queue<string>>();
            _model = GetConnectionFactory(rabbitUser, rabbitPassword, rabbitHostname)
                .CreateConnection()
                .CreateModel();
        }

        public bool SendMessage(string queueName, string message)
        {
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
            message = "";
            if (!_queues.ContainsKey(queueName))
            {
                SetConsumer(queueName);
                Thread.Sleep(250);
            }

            lock(_queues)
            {
                Queue<string> queue = _queues[queueName];
                if (queue.Count == 0)
                {
                    return false;
                }

                message = queue.Dequeue();
                return true;
            }
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
                _queues[queueName].Enqueue(message);
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
