using DataContracts.Common;

namespace SecretaryProblemWebClient;

public class Princess
{
    public const int ContendersTotalNumber = 100;
    private const double ContendersToSkipFactor = 1 / Math.E;
    private const double ContendersLimitFactor = 1;

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

    private async void SkipForeverByFactor()
    {
        _currentContenderNumber = 0;
        for (var i = 0; i < ContendersTotalNumber * ContendersToSkipFactor; i++)
        {
            RememberVisitedContender(await _webClient.GetNextContender());
            _currentContenderNumber += 1;
        }
    }

    public async Task<Contender?> MakeChoice()
    {
        SkipForeverByFactor();

        while (_currentContenderNumber < ContendersTotalNumber)
        {
            var contender = await _webClient.GetNextContender();
            var contendersCounter = 0;
            foreach (var visitedContender in _visitedContenders)
            {
                var friendAnswer = await _webClient.CompareContenders(
                    firstContender: contender,
                    secondContender: visitedContender);
                if (friendAnswer)
                {
                    contendersCounter++;
                }
            }

            if (contendersCounter >= ContendersTotalNumber * ContendersToSkipFactor * ContendersLimitFactor)
            {
                contendersCounter = 0;
                foreach (var bestContender in _bestContenders)
                {
                    var friendAnswer = await _webClient.CompareContenders(
                        firstContender: contender,
                        secondContender: bestContender);
                    if (friendAnswer)
                    {
                        contendersCounter++;
                    }
                }

                if (_bestContenders.Count >= 3 && contendersCounter == _bestContenders.Count)
                {
                    var rank = await _webClient.GetFinalContenderRank();
                    return new RatingContender(
                        surname: contender.Surname,
                        name: contender.Name,
                        rating: rank
                    );
                }

                _bestContenders.Add(contender);
            }

            RememberVisitedContender(contender);

            _currentContenderNumber += 1;
        }

        return null;
    }
}