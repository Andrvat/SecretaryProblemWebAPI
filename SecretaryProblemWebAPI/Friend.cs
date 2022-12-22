namespace SecretaryProblemWebAPI;

public class Friend
{
    private readonly List<RatingContender> _visitedContenders = new();

    public void NotifyAboutContender(RatingContender contender)
    {
        _visitedContenders.Add(contender);
    }

    public bool ReplyToComparison(Contender newContender, Contender oldContender)
    {
        var foundContender = _visitedContenders.Find(contender =>
            contender.Surname.Equals(oldContender.Surname) &&
            contender.Name.Equals(oldContender.Name) &&
            contender.Patronymic.Equals(oldContender.Patronymic));
        if (foundContender is null)
        {
            throw new InvalidOperationException(
                $"Seems like contender {oldContender.GetFullName()} does not visit princess before!");
        }

        var ratingContender = (RatingContender)newContender;
        return ratingContender.Rating > foundContender.Rating;
    }

    public void Reset()
    {
        _visitedContenders.Clear();
    }
}