using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SecretaryProblemWebClient;

public class TaskSimulator : IHostedService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly Task _makeChoiceTask;

    private Princess _princess;
    private readonly AttemptsNumberProvider _attemptsNumberProvider;

    public TaskSimulator(IServiceScopeFactory serviceScopeFactory,
        IHostApplicationLifetime applicationLifetime,
        Princess princess, AttemptsNumberProvider attemptsNumberProvider)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _applicationLifetime = applicationLifetime;
        _princess = princess;
        _attemptsNumberProvider = attemptsNumberProvider;
        _makeChoiceTask = new Task(Simulate);
    }

    private void Simulate()
    {
        try
        {
            var happiness = _attemptsNumberProvider.AttemptNumber switch
            {
                null => GetAverageHappiness(),
                _ => GetHappinessByAttempt()
            };
            Console.WriteLine("________");
            Console.WriteLine(happiness);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Data);
            Console.WriteLine(e.Message);
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }

    private double GetAverageHappiness()
    {
        double totalHappiness = 0;
        for (var i = 0; i < Princess.ContendersTotalNumber; ++i)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _attemptsNumberProvider.AttemptNumber = i;
            _princess = scope.ServiceProvider.GetService<Princess>()!;
            var happiness = GetHappinessByAttempt();
            totalHappiness += happiness;
            Console.WriteLine($"{i} : {happiness}");
        }

        return totalHappiness / Princess.ContendersTotalNumber;
    }

    private double GetHappinessByAttempt()
    {
        var princessChoice = _princess.MakeChoice();
        return HappinessEstimator.EstimatePrincessHappiness(princessChoice);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _makeChoiceTask.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}