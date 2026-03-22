using NotificationService.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NotificationService.Services {
    public class RabbitMQConsumer : BackgroundService {
        private readonly string _rabbitUrl;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQConsumer(IConfiguration config) {
            _rabbitUrl = config.GetValue<string>("RabbitMQ:Url");
        }

        /// <summary>
        /// Initialize the connection and channel once.
        /// </summary>
        public async Task InitAsync(CancellationToken stoppingToken) {
            var factory = new ConnectionFactory { Uri = new Uri(_rabbitUrl) };

            for (int attempt = 1; attempt <= 10; attempt++) {
                try {
                    _connection = await factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();

                    // Declare exchange & queue
                    await _channel.ExchangeDeclareAsync("user_exchange", ExchangeType.Fanout, durable: true);
                    await _channel.QueueDeclareAsync(
                        queue: "notification_queue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false
                    );
                    await _channel.QueueBindAsync("notification_queue", "user_exchange", "");

                    Console.WriteLine("[RabbitMQConsumer] Connection and channel initialized!");
                    return;
                } catch (Exception ex) {
                    Console.WriteLine($"[RabbitMQConsumer] RabbitMQ not ready, retry {attempt}/10: {ex.Message}");
                    await Task.Delay(2000, stoppingToken);
                }
            }

            throw new Exception("Failed to connect to RabbitMQ after multiple attempts");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            if (_channel == null) {
                throw new InvalidOperationException("RabbitMQConsumer channel is not initialized. Call InitAsync before starting the service.");
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) => {
                try {
                    var body = ea.Body.ToArray();
                    var userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(body);

                    Console.WriteLine($"[NotificationService] New user created: {userEvent?.Email}");

                    // Simulate processing (e.g., send email)
                    await Task.Delay(50);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                } catch (Exception ex) {
                    Console.WriteLine($"[RabbitMQConsumer] Error processing message: {ex.Message}");
                }
            };

            // Start consuming messages (autoAck false since we manually ack)
            await _channel.BasicConsumeAsync(
                queue: "notification_queue",
                autoAck: false,
                consumer: consumer
            );

            Console.WriteLine("[RabbitMQConsumer] Consumer started!");
        }

        public override async Task StopAsync(CancellationToken cancellationToken) {
            if (_channel != null) {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection != null) {
                await _connection.CloseAsync();
                _connection.Dispose();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}