using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailConsumer
{
    /*
    * Demo Consumer Service
    * ---------------------
    * Logs to console as a placeholder for actual email sending logic.
    * Successful sending of email requires control of domain DNS settings.
    * This demo project does not have a domain, so attempting to send real emails would fail.
    * Demonstrates event-driven message consumption from RabbitMQ queue named "email_notifications".
    */


    internal class Program
    {
        static async Task Main(string[] args)
        {
            var consumerService = await EmailConsumerService.CreateAsync();
            await consumerService.StartConsumingAsync();

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();

            await consumerService.DisposeAsync();
        }
    }

    public class EmailConsumerService : IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        private EmailConsumerService(IConnection connection, IChannel channel)
        {
            _connection = connection;
            _channel = channel;
        }

        public static async Task<EmailConsumerService> CreateAsync(string hostName = "localhost")
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "email_notifications",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            return new EmailConsumerService(connection, channel);
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken = default)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[EmailConsumer] Received message → {message}");
                await Task.Yield(); 
            };

            await _channel.BasicConsumeAsync(
                queue: "email_notifications",
                autoAck: true,
                consumer: consumer,
                cancellationToken: cancellationToken
            );

            Console.WriteLine("[EmailConsumer] Listening for messages...");
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
        }
    }
}
