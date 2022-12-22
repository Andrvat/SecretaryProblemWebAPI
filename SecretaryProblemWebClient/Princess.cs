using SecretaryProblemWebAPI;

namespace SecretaryProblemWebClient;

public class Princess
{
    public const int ContendersTotalNumber = 100;
    private const double ContendersToSkipFactor = 1 / Math.E;
    private const double ContendersLimitFactor = 0.96;

    private int? _currentContenderNumber;
    private readonly List<Contender> _visitedContenders = new();

    private readonly List<Contender> _bestContenders = new();

    private PrincessHttpWebClient _webClient;

    public Princess(PrincessHttpWebClient webClient)
    {
        _webClient = webClient;
    }

    private void RememberVisitedContender(Contender contender)
    {
        _visitedContenders.Add(contender);
    }

    private void SkipForeverByFactor()
    {
        _currentContenderNumber = 0;
        for (var i = 0; i < ContendersTotalNumber * ContendersToSkipFactor; i++)
        {
            RememberVisitedContender(_webClient.GetNextContender());
            _currentContenderNumber += 1;
        }
    }

    public Contender? MakeChoice()
    {
        SkipForeverByFactor();

        while (_currentContenderNumber < ContendersTotalNumber)
        {
            var contender = _webClient.GetNextContender();
            var contendersCounter = 0;
            foreach (var visitedContender in _visitedContenders)
            {
                var friendAnswer = _webClient.CompareContenders(
                    firstContender: contender,
                    secondContender: visitedContender);
                if (friendAnswer)
                {
                    contendersCounter++;
                }
            }

            if (contendersCounter >= ContendersTotalNumber * ContendersToSkipFactor * ContendersLimitFactor)
            {
                // contendersCounter = 0;
                // foreach (var bestContender in _bestContenders)
                // {
                //     var friendAnswer = _context.Friend.ReplyToComparison(
                //         newContender: contender,
                //         oldContender: bestContender);
                //     if (friendAnswer)
                //     {
                //         contendersCounter++;
                //     }
                // }
                //
                // if (_bestContenders.Count > 7 && _bestContenders.Count - 4 <= contendersCounter  && contendersCounter <= _bestContenders.Count - 3)
                // {
                //     return contender;
                // }
                //
                // _bestContenders.Add(contender);
                var rank = _webClient.GetFinalContenderRank();
                return new RatingContender(
                    surname: contender.Surname,
                    name: contender.Name,
                    patronymic: contender.Patronymic,
                    rating: rank
                );
            }

            RememberVisitedContender(contender);

            _currentContenderNumber += 1;
        }

        return null;
    }
}