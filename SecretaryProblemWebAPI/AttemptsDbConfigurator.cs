using DataContracts.Entities;
using SecretaryProblemWebAPI.Generators;

namespace SecretaryProblemWebAPI;

public class AttemptsDbConfigurator
{
    public const int AttemptsNumber = 100;
    public const int ContendersNumber = 100;
    private const int AttemptRecordsNumber = ContendersNumber * AttemptsNumber;

    private AttemptsDbContext _context;
    private ContendersFileGenerator _generator;

    public AttemptsDbConfigurator(AttemptsDbContext context, ContendersFileGenerator generator)
    {
        _context = context;
        _generator = generator;
    }

    public void ConfigureAttempts()
    {
        if (ContendersNumber != _context.RatingContenderEntities.Count())
        {
            _context.AttemptRecordEntities.RemoveRange(_context.AttemptRecordEntities);
            _context.RatingContenderEntities.RemoveRange(_context.RatingContenderEntities);
            _context.SaveChanges();

            FillRatingContenders();
            FillAttemptRecords();
        }
        else if (AttemptRecordsNumber != _context.AttemptRecordEntities.Count())
        {
            _context.AttemptRecordEntities.RemoveRange(_context.AttemptRecordEntities);
            _context.SaveChanges();

            FillAttemptRecords();
        }
    }

    private void FillAttemptRecords()
    {
        for (var attempt = 0; attempt < AttemptsNumber; ++attempt)
        {
            _generator.CreateContenders();
            var contenders = _generator.GetContenders();
            for (var num = 0; num < contenders.Count; ++num)
            {
                var ratingContenderEntity = _context.RatingContenderEntities.First(contender =>
                    Equals(contender.Rating, ((RatingContender)contenders[num]).Rating));
                _context.AttemptRecordEntities.Add(new AttemptRecordEntity
                {
                    AttemptNumber = attempt,
                    ContenderEntity = ratingContenderEntity
                });
            }
        }

        _context.SaveChanges();
    }

    private void FillRatingContenders()
    {
        _generator.CreateContenders();
        var contenders = _generator.GetContenders();
        foreach (var contender in contenders)
        {
            var ratingContender = (RatingContender)contender;
            _context.RatingContenderEntities.Add(new RatingContenderEntity
            {
                Surname = ratingContender.Surname,
                Name = ratingContender.Name,
                Patronymic = ratingContender.Patronymic,
                Rating = ratingContender.Rating
            });
        }

        _context.SaveChanges();
    }
}