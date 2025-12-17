using EmailConsumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailConsumer.Services
{
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
                var email = JsonSerializer.Deserialize<Email>(json);

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
