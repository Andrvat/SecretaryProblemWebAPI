namespace DataContracts.Common;

public class RatingContender : Contender
{
    public int Rating { get; }

    public RatingContender(string surname, string name, int rating)
        : base(surname, name)
    {
        Rating = rating;
    }
}