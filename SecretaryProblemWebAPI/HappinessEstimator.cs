using DataContracts.Common;

namespace SecretaryProblemWebAPI;

public static class HappinessEstimator
{
    public static int EstimatePrincessHappiness(Contender? contender)
    {
        var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var path = Path.Combine(docPath, "file.txt");
        if (contender == null)
        {
            File.AppendAllText(path, "10\n");
            return 10;
        }

        var ratingContender = (RatingContender)contender;
        File.AppendAllText(path, ratingContender.Rating.ToString() + "\n");
        return ratingContender.Rating switch
        {
            99 => 20,
            97 => 50,
            95 => 100,
            _ => 0
        };
    }
}