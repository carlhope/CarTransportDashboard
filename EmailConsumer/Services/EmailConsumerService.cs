
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
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
                // currently uses RecipientUserId. This would typically be used to look up the user's email address.
                // producer project currently uses in memory database, so is inaccessible from this demo consumer project.

                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var data = JsonSerializer.Deserialize<Email>(json);
                var template = EmailTemplateFactory.Create(data.EmailType);
                Console.WriteLine(template.GenerateMessage(data));
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
