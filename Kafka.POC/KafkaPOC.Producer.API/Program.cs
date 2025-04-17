using System.Net;
using Confluent.Kafka;
using FluentValidation;
using KafkaPOC.Producer.API.Middlewares;
using KafkaPOC.Producer.Application.Commands.CreateOrder;
using KafkaPOC.Producer.Application.Interfaces.OutboxEventLog;
using KafkaPOC.Producer.Domain.Repositories;
using KafkaPOC.Producer.Domain.Repositories.Order;
using KafkaPOC.Producer.Domain.Repositories.OutboxEvent;
using KafkaPOC.Producer.Infrastructure.Kafka;
using KafkaPOC.Producer.Infrastructure.Persistence;
using KafkaPOC.Producer.Infrastructure.Repositories;
using KafkaPOC.Producer.Infrastructure.Repositories.Order;
using KafkaPOC.Producer.Infrastructure.Repositories.OutboxEvent;
using KafkaPOC.Producer.Infrastructure.Repositories.OutboxEventLog;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Logger
Log.Logger = new LoggerConfiguration()
.WriteTo.File("Logs/errors.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

builder.Host.UseSerilog();

#endregion

#region DB Context and Repo
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOutboxEventRepository, OutboxEventRepository>();
builder.Services.AddScoped<IOutboxEventLogRepository, OutboxEventLogRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


#endregion

#region Outbox Publisher and Ack Consumer

builder.Services.AddSingleton<IProducer<Null, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9092",
        Acks = Acks.All,

        SecurityProtocol = SecurityProtocol.Plaintext,
        SocketTimeoutMs = 10000,  
        RequestTimeoutMs = 1000 ,
        RetryBackoffMs = 1000 ,
        MessageTimeoutMs = 10000, 


    };

    return new ProducerBuilder<Null, string>(config).Build();
});
builder.Services.AddHostedService<OutboxPublisher>();
builder.Services.AddHostedService<AckConsumer>();

#endregion

#region MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200") 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});




var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
