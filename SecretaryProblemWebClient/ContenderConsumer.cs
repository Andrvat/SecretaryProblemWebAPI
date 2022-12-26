using MassTransit;
using Nsu.PeakyBride.DataContracts;

namespace SecretaryProblemWebClient;

public class ContenderConsumer : IConsumer<Contender>
{
    private readonly ContenderConsumerService _contenderConsumerService;
    
    public ContenderConsumer(ContenderConsumerService contenderConsumerService)
    {
        _contenderConsumerService = contenderConsumerService;
    }

    public Task Consume(ConsumeContext<Contender> context)
    {
        var name = context.Message.Name;
        Console.WriteLine($"Consumer notifies about new name: {name}");
        _contenderConsumerService.SetNextContender(name);
        return Task.CompletedTask;
    }
}
