using DataContracts.Dtos;

namespace SecretaryProblemWebClient;

public class ContenderConsumerService
{
    private ContenderFullNameDto? _contender;
    private TaskCompletionSource<ContenderFullNameDto>? _tcs;

    public void SetNextContender(ContenderFullNameDto contender)
    {
        _contender = contender;
        _tcs?.SetResult(contender);
        _tcs = null;
    }

    public async Task<ContenderFullNameDto> AwaitContender()
    {
        if (_contender != null)
        {
            return _contender;
        }

        _tcs = new TaskCompletionSource<ContenderFullNameDto>();
        return await _tcs.Task;
    }
}