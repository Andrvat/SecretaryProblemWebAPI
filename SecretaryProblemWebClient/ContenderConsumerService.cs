using DataContracts.Dtos;

namespace SecretaryProblemWebClient;

public class ContenderConsumerService
{
    private string? _contenderFullName;
    private TaskCompletionSource<string>? _tcs;

    public void SetNextContender(string? contenderFullName)
    {
        _contenderFullName = contenderFullName;
        _tcs?.SetResult(contenderFullName!);
        _tcs = null;
        // Console.WriteLine("Consumer service set result");
        // Console.Out.Flush();
    }

    public async Task<string> AwaitContender()
    {
        // Console.WriteLine("Consumer service await contender");
        // await Console.Out.FlushAsync();
        if (_contenderFullName != null)
        {
            return _contenderFullName;
        }

        _tcs = new TaskCompletionSource<string>();
        return await _tcs.Task;
    }
}