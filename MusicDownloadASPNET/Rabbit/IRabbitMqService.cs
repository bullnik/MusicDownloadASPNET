namespace MusicDownloadASPNET.Rabbit
{
    public interface IRabbitMqService
    {
        public bool SendMessage(string queueName, string message);

        public bool TryReceiveMessage(string queueName, out string message);
    }
}
