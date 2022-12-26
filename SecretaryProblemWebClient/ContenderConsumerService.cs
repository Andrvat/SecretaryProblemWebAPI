using DataContracts.Dtos;

namespace SecretaryProblemWebClient;

public class ContenderConsumerService
{
    private string? _contender;
    private TaskCompletionSource<string>? _tcs;

    public void SetNextContender(string contender)
    {
        _contender = contender;
        _tcs?.SetResult(contender);
        _tcs = null;
        Console.WriteLine("Consumer service set result");
        Console.Out.Flush();
    }

    public async Task<string> AwaitContender()
    {
        Console.WriteLine("Consumer service await contender");
        await Console.Out.FlushAsync();
        if (_contender != null)
        {
            return _contender;
        }

        _tcs = new TaskCompletionSource<string>();
        return await _tcs.Task;
    }
}