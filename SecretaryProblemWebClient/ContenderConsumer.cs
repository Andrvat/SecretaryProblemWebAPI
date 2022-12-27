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
        if (name is null)
        {
            name = "ABC";
        }
        _contenderConsumerService.SetNextContender(name);
        return Task.CompletedTask;
    }
}
