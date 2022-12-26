using DataContracts.MassTransit;
using MassTransit;

namespace SecretaryProblemWebClient;

public class ContenderCreatedConsumer : IConsumer<IContenderCreated>
{
    private readonly ContenderConsumerService _contenderConsumerService;
    
    public ContenderCreatedConsumer(ContenderConsumerService contenderConsumerService)
    {
        _contenderConsumerService = contenderConsumerService;
    }

    public Task Consume(ConsumeContext<IContenderCreated> context)
    {
        var contenderFullNameDto = context.Message.FullNameDto;
        _contenderConsumerService.SetNextContender(contenderFullNameDto);
        return Task.CompletedTask;
    }
}