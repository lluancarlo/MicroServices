﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MassTransit;
using CoordinatorService.DB;
using CoordinatorService.Consumers;
using CoordinatorService.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Add Services
builder.Services.AddHostedService<ShiftService>();
builder.Services.AddHostedService<CoordinatorMainService>();

// Configure DB Context
builder.Services.AddDbContext<DatabaseContext>(op =>
    op.UseInMemoryDatabase(builder.Configuration.GetConnectionString("Database")));

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        cfg.ConfigureEndpoints(ctx);
        cfg.ReceiveEndpoint("session-queue", x =>
        {
            x.Consumer<SessionConsumer>(ctx);
            x.Bind("ChatAPI");
        });
    });
});
builder.Services.AddScoped<SessionConsumer>();


IHost host = builder.Build();
host.Run();
