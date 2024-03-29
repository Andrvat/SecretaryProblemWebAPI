﻿using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SecretaryProblemWebClient;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
        {
            services.AddSingleton(_ => new CliArgumentsParser(args));
            services.AddHostedService<TaskSimulator>();
            services.AddSingleton<AttemptsNumberProvider>();
            services.AddScoped<PrincessHttpWebClient>();
            services.AddScoped<Princess>();
            services.AddScoped<ContenderCreatedConsumer>();
            services.AddSingleton<ContenderConsumerService>();
            services.AddHttpClient();

            services.AddOptions<MassTransitHostOptions>().Configure(
                options => { options.WaitUntilStarted = true; });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ContenderCreatedConsumer>();
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-ip-local"]!), h =>
                    {
                        h.Username(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-user-local"]!);
                        h.Password(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-password-local"]!);
                    });

                    cfg.ConfigureEndpoints(ctx);
                });
            });
        });
    }
}