using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nsu.PeakyBride.DataContracts;

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
            services.AddScoped<ContenderConsumer>();
            services.AddSingleton<ContenderConsumerService>();
            services.AddHttpClient();

            services.AddOptions<MassTransitHostOptions>().Configure(
                options => { options.WaitUntilStarted = true; });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ContenderConsumer>();
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-ip-competition"]!), h =>
                    {
                        h.Username(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-user-competition"]!);
                        h.Password(System.Configuration.ConfigurationManager.AppSettings["rabbitmq-password-competition"]!);
                    });

                    cfg.ConfigureEndpoints(ctx);
                });
            });
        });
    }
}