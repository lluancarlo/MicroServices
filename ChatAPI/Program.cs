using ChatAPI.Domain.Publishers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddControllers();
builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("dev", false));
        cfg.Publish<SessionMessage>(x =>
        {
            x.Durable = true;
            x.AutoDelete = false;
            x.ExchangeType = "fanout";
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
