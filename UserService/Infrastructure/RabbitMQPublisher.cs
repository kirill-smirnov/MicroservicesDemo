using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UserService.Domain.Events;

namespace UserService.Infrastructure {
    public class RabbitMQPublisher : IAsyncDisposable {
        private IConnection _connection;
        private IChannel _channel;

        public static async Task<RabbitMQPublisher> InitAsync(string url, int maxRetries = 10, int delayMs = 2000) {
            var result = new RabbitMQPublisher();
            var factory = new ConnectionFactory() { Uri = new Uri(url) };

            for (int attempt = 1; attempt <= maxRetries; attempt++) {
                try {
                    // Try to connect and create channel
                    result._connection = await factory.CreateConnectionAsync();
                    result._channel = await result._connection.CreateChannelAsync();

                    // Declare the exchange
                    await result._channel.ExchangeDeclareAsync("user_exchange", ExchangeType.Fanout, durable: true);

                    Console.WriteLine($"Connected to RabbitMQ on attempt {attempt}");
                    return result;
                } catch (Exception ex) {
                    Console.WriteLine($"RabbitMQ not ready (attempt {attempt}/{maxRetries}): {ex.Message}");

                    if (attempt == maxRetries) {
                        throw new Exception("Could not connect to RabbitMQ after multiple retries", ex);
                    }

                    // Wait before retrying
                    await Task.Delay(delayMs);
                }
            }

            // This line should never be reached
            throw new Exception("Unexpected error initializing RabbitMQPublisher");
        }

        public async Task PublishUserCreatedAsync(UserCreatedEvent @event) {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

            var props = new BasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = DeliveryModes.Persistent;

            await  _channel.BasicPublishAsync("user_exchange", "", true, props, body, CancellationToken.None);
        }

        public async ValueTask DisposeAsync() {
            await _channel?.CloseAsync();
            await _connection?.CloseAsync();

            await Task.CompletedTask;
        }
    }
}
