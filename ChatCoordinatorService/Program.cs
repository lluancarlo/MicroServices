using ChatCoordinatorService.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddControllers();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SessionConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        // cfg.PrefetchCount = 32; // applies to all receive endpoints
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        cfg.ReceiveEndpoint("session-queue", e =>
        {
            // e.ConcurrentMessageLimit = 28; // only applies to this endpoint
            e.ConfigureConsumer<SessionConsumer>(ctx);
        });
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();