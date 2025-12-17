using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
    public record EmailMessage(string RecipientUserId, EmailType EmailType, string SenderUserId);
    public enum EmailType
    {
        JobAccepted,
        JobAssigned,
        PasswordReset,
        AccountCreated,
        Unknown
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
                var json = Encoding.UTF8.GetString(body);

                // Deserialize JSON back into a typed object
                // currently uses RecipientUserId. This would typically be used to look up the user's email address.
                // primary project currently uses in memory database, so is inaccessible from this demo consumer project.
                var email = JsonSerializer.Deserialize<EmailMessage>(json);

                string template = email.EmailType switch
                {
                    EmailType.JobAccepted => $"User {email.RecipientUserId} has accepted the job.",
                    EmailType.JobAssigned => $"User {email.RecipientUserId} has been assigned a new job by {email.SenderUserId}.",
                    EmailType.PasswordReset => $"User {email.RecipientUserId} requested a password reset.",
                    EmailType.AccountCreated => $"Welcome {email.RecipientUserId}, your account has been created.",
                    _ => $"Unknown email type for {email.RecipientUserId}."
                };

                Console.WriteLine($"[EmailConsumer] Generated email → {template}");


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
