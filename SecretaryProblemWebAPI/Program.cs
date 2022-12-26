using DataContracts.Common;
using MassTransit;
using SecretaryProblemWebAPI;
using SecretaryProblemWebAPI.Generators;
using SecretaryProblemWebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AttemptsDbContext>();
builder.Services.AddSingleton<ContendersFileGenerator>();
builder.Services.AddSingleton<AttemptsDbConfigurator>();

builder.Services.AddSingleton<AttemptsNumberProvider>();
builder.Services.AddSingleton<ContendersDbGenerator>();

builder.Services.AddSingleton<Hall>();
builder.Services.AddSingleton<Friend>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host(new Uri(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-ip-local"]!), h =>
        {
            h.Username(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-user-local"]!);
            h.Password(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-password-local"]!);
        });
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<HandleHttpRequestMiddleware>();
app.MapControllers();

app.Run();