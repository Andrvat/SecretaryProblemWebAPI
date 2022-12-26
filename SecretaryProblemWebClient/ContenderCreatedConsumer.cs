using DataContracts.MassTransit;
using MassTransit;
using Nsu.PickyBride.DataContracts;

namespace SecretaryProblemWebClient;

public class ContenderCreatedConsumer : IConsumer<Contender>
{
    private readonly ContenderConsumerService _contenderConsumerService;
    
    public ContenderCreatedConsumer(ContenderConsumerService contenderConsumerService)
    {
        _contenderConsumerService = contenderConsumerService;
    }

    public Task Consume(ConsumeContext<Contender> context)
    {
        var contenderFullNameDto = context.Message.Name;
        _contenderConsumerService.SetNextContender(contenderFullNameDto);
        return Task.CompletedTask;
    }
}