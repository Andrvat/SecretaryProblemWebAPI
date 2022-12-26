using DataContracts.Common;

namespace SecretaryProblemWebAPI;

public class Friend
{
    private readonly List<RatingContender> _visitedContenders = new();

    public void NotifyAboutContender(RatingContender contender)
    {
        _visitedContenders.Add(contender);
    }

    public bool ReplyToComparison(string? firstContenderFullName, string? secondContenderFullName)
    {
        var firstContender = _visitedContenders.Find(contender => contender.GetFullName() == firstContenderFullName);
        var secondContender = _visitedContenders.Find(contender => contender.GetFullName() == secondContenderFullName);
        return CompareContenders(firstContender, secondContender);
    }

    private static bool CompareContenders(Contender? firstContender, Contender? secondContender)
    {
        if (firstContender is null || secondContender is null)
        {
            throw new InvalidOperationException("Some of contenders didn't visit princess before");
        }

        return ((RatingContender)firstContender).Rating > ((RatingContender)secondContender).Rating;
    }

    public void Reset()
    {
        _visitedContenders.Clear();
    }
}