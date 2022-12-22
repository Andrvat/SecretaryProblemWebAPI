using DataContracts.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace SecretaryProblemWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class HallController : ControllerBase
{
    private readonly ILogger<HallController> _logger;

    private readonly Hall _hall;
    private readonly Friend _friend;

    public HallController(ILogger<HallController> logger, Hall hall, Friend friend)
    {
        _logger = logger;
        _hall = hall;
        _friend = friend;
    }

    [HttpPost("{attemptNumber:int}/next")]
    public ContenderFullNameDto GetNextContenderForGivenAttempt(int attemptNumber, [FromQuery] int session)
    {
        if (_hall.GetQueueCount() == 0)
        {
            return new ContenderFullNameDto
            {
                Name = null
            };
        }

        var currentContender = (RatingContender)_hall.GetNextContender();
        _friend.NotifyAboutContender(currentContender);

        return new ContenderFullNameDto()
        {
            Name = currentContender.GetFullName()
        };
    }

    [HttpPost("{attemptNumber:int}/select")]
    public ContenderRankDto GetContenderRankFromAttempt(int attemptNumber, [FromQuery] int session)
    {
        if (_hall.GetLastContender() is not RatingContender contender)
        {
            return new ContenderRankDto
            {
                Rank = null
            };
        }

        return new ContenderRankDto
        {
            Rank = contender.Rating
        };
    }
}