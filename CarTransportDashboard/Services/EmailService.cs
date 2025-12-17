using CarTransportDashboard.Services.Interfaces;

namespace CarTransportDashboard.Services
{
    using RabbitMQ.Client;
    // EmailService.cs (producer)
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class EmailService : IAsyncDisposable, IEmailService
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        // Private constructor
        private EmailService(IConnection connection, IChannel channel)
        {
            _connection = connection;
            _channel = channel;
        }

        // Async factory method
        public static async Task<EmailService> CreateAsync(string hostName = "localhost")
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

            return new EmailService(connection, channel);
        }


        // SendEmail method
        public async Task SendEmailAsync(string recipientUserId, string emailType, string senderUserId)
        {
            if (string.IsNullOrEmpty(recipientUserId)) return;

            var payload = JsonSerializer.Serialize(new { recipientUserId, emailType, senderUserId });
            var bytes = Encoding.UTF8.GetBytes(payload);

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "email_notifications", // must match QueueDeclare
                mandatory: false,
                body: bytes,
                cancellationToken: CancellationToken.None
            );

            Console.WriteLine($"[EmailService] Published email message → To: {recipientUserId}, Subject: {emailType}");
        }


        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
        }
    }

}
