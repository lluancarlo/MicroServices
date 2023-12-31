using DomainLib.Contracts;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

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
        cfg.ConfigureEndpoints(ctx);
        cfg.Message<CreateSessionMessage>(x => x.SetEntityName("create-session"));
        cfg.Message<PollSessionMessage>(x => x.SetEntityName("poll-session"));
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
