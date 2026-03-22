
using UserService.Infrastructure;

namespace UserService {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var rabbitMqPublisher = await RabbitMQPublisher.InitAsync(builder.Configuration["RabbitMQ:Url"]);
            builder.Services.AddSingleton(rabbitMqPublisher);


            // builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(5000));

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
