using DataContracts.Common;

namespace SecretaryProblemWebClient;

public static class HappinessEstimator
{
    public static int EstimatePrincessHappiness(Contender? contender)
    {
        var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var path = Path.Combine(docPath, "file.txt");
        if (contender == null)
        {
            File.AppendAllText(path, "10,\n");
            return 10;
        }

        var ratingContender = (RatingContender)contender;
        File.AppendAllText(path, $"{ratingContender.Rating}, {ratingContender.GetFullName()}\n");
        return ratingContender.Rating switch
        {
            100 => 20,
            98 => 50,
            96 => 100,
            _ => 0
        };
    }
}