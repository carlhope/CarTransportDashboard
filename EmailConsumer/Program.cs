using EmailConsumer.Services;

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
}
