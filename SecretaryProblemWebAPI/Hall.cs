using System.Diagnostics;
using DataContracts.Common;

namespace SecretaryProblemWebAPI;

public class Hall
{
    private readonly Queue<Contender> _contendersQueue = new();

    private Contender? _lastContender;
    
    private readonly Friend _friend;

    public Hall(Friend friend)
    {
        _friend = friend;
    }

    public void NotifyFriendAboutReset()
    {
        _friend.Reset();
    }

    public void InviteContenders(List<Contender>? contenders)
    {
        Debug.Assert(contenders != null, nameof(contenders) + " != null");
        Reset();
        foreach (var contender in contenders)
        {
            _contendersQueue.Enqueue(contender);
        }
    }

    private void Reset()
    {
        _contendersQueue.Clear();
        _lastContender = null;
    }

    public Contender GetNextContender()
    {
        var contender = _contendersQueue.Dequeue();
        _lastContender = contender;
        return _lastContender;
    }

    public int GetQueueCount()
    {
        return _contendersQueue.Count;
    }

    public Contender? GetLastContender()
    {
        return _lastContender;
    }
}