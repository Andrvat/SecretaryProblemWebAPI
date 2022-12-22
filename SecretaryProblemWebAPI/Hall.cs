using System.Diagnostics;

namespace SecretaryProblemWebAPI;

public class Hall
{
    private readonly Queue<Contender> _contendersQueue = new();
    public int QueueInitialCount { get; private set; }

    public void InviteContenders(List<Contender>? contenders)
    {
        Debug.Assert(contenders != null, nameof(contenders) + " != null");
        foreach (var contender in contenders)
        {
            _contendersQueue.Enqueue(contender);
        }

        QueueInitialCount = _contendersQueue.Count;
    }

    public Contender GetNextContender()
    {
        return _contendersQueue.Dequeue();
    }

    public int GetQueueCount()
    {
        return _contendersQueue.Count;
    }
}