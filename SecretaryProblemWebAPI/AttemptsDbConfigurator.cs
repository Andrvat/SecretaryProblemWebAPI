using DataContracts.Common;
using DataContracts.Entities;
using SecretaryProblemWebAPI.Generators;

namespace SecretaryProblemWebAPI;

public class AttemptsDbConfigurator
{
    public const int AttemptsNumber = 100;
    public const int ContendersNumber = 100;
    private const int AttemptRecordsNumber = ContendersNumber * AttemptsNumber;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ContendersFileGenerator _generator;

    public AttemptsDbConfigurator(IServiceScopeFactory scopeFactory, ContendersFileGenerator generator)
    {
        _scopeFactory = scopeFactory;
        _generator = generator;
    }

    public void ConfigureAttempts()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AttemptsDbContext>();
        if (ContendersNumber != dbContext.RatingContenderEntities.Count())
        {
            dbContext.AttemptRecordEntities.RemoveRange(dbContext.AttemptRecordEntities);
            dbContext.RatingContenderEntities.RemoveRange(dbContext.RatingContenderEntities);
            dbContext.SaveChanges();

            FillRatingContenders(context: dbContext);
            FillAttemptRecords(context: dbContext);
        }
        else if (AttemptRecordsNumber != dbContext.AttemptRecordEntities.Count())
        {
            dbContext.AttemptRecordEntities.RemoveRange(dbContext.AttemptRecordEntities);
            dbContext.SaveChanges();

            FillAttemptRecords(context: dbContext);
        }
    }

    private void FillAttemptRecords(AttemptsDbContext context)
    {
        for (var attempt = 0; attempt < AttemptsNumber; ++attempt)
        {
            _generator.CreateContenders();
            var contenders = _generator.GetContenders();
            for (var num = 0; num < contenders.Count; ++num)
            {
                var ratingContenderEntity = context.RatingContenderEntities.First(contender =>
                    Equals(contender.Rating, ((RatingContender)contenders[num]).Rating));
                context.AttemptRecordEntities.Add(new AttemptRecordEntity
                {
                    AttemptNumber = attempt,
                    ContenderEntity = ratingContenderEntity
                });
            }
        }

        context.SaveChanges();
    }

    private void FillRatingContenders(AttemptsDbContext context)
    {
        _generator.CreateContenders();
        var contenders = _generator.GetContenders();
        foreach (var contender in contenders)
        {
            var ratingContender = (RatingContender)contender;
            context.RatingContenderEntities.Add(new RatingContenderEntity
            {
                Surname = ratingContender.Surname,
                Name = ratingContender.Name,
                Rating = ratingContender.Rating
            });
        }

        context.SaveChanges();
    }
}