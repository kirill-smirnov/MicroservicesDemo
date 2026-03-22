using NotificationService.Services;

namespace NotificationService {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = Host.CreateApplicationBuilder(args);

            // Add HostedService (RabbitMQConsumer)
            builder.Services.AddSingleton<RabbitMQConsumer>(sp =>
            {
                var consumer = new RabbitMQConsumer(builder.Configuration);
                // Initialize the connection/channel before starting the hosted service
                consumer.InitAsync(CancellationToken.None).GetAwaiter().GetResult();
                return consumer;
            });
            builder.Services.AddHostedService(sp => sp.GetRequiredService<RabbitMQConsumer>());

            var host = builder.Build();

            host.Run();
        }
    }
}
