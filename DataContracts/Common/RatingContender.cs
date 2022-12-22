namespace SecretaryProblemWebAPI;

public class RatingContender : Contender
{
    public int Rating { get; }

    public RatingContender(string surname, string name, string patronymic, int rating)
        : base(surname, name, patronymic)
    {
        Rating = rating;
    }
}