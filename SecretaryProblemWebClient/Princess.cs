using DataContracts.Common;

namespace SecretaryProblemWebClient;

public class Princess
{
    public const int ContendersTotalNumber = 100;
    private const double ContendersToSkipFactor = 1 / Math.E;

    private int? _currentContenderNumber;
    
    private Contender? _bestContender;
    private int _bestContendersCounter;
    

    private readonly PrincessHttpWebClient _webClient;

    public Princess(PrincessHttpWebClient webClient)
    {
        _webClient = webClient;
    }

    private void SkipForeverByFactor()
    {
        _currentContenderNumber = 0;
        for (var i = 0; i < ContendersTotalNumber * ContendersToSkipFactor; i++)
        {
            var contender = _webClient.GetNextContender()!;
            if (_bestContender is null)
            {
                _bestContender = contender;
                _currentContenderNumber += 1;
                continue;
            }

            var friendAnswer = _webClient.CompareContenders(
                firstContender: contender,
                secondContender: _bestContender);
            if (friendAnswer)
            {
                _bestContender = contender;
            }
            _currentContenderNumber += 1;
        }
    }

    public Contender? MakeChoice()
    {
        SkipForeverByFactor();

        while (_currentContenderNumber <= ContendersTotalNumber)
        {
            var contender = _webClient.GetNextContender();
            if (contender is null)
            {
                return null;
            }
            
            var friendAnswer = _webClient.CompareContenders(
                firstContender: contender,
                secondContender: _bestContender!);
            if (friendAnswer)
            {
                _bestContender = contender;
                _bestContendersCounter += 1;
                if (_bestContendersCounter == 3)
                {
                    var rank = _webClient.GetFinalContenderRank();
                    return new RatingContender(
                        surname: _bestContender.Surname,
                        name: _bestContender.Name,
                        rating: rank
                    );
                }
            }

            _currentContenderNumber += 1;
        }

        return null;
    }
}