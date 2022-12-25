using DataContracts.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace SecretaryProblemWebAPI.Controllers;

[ApiController]
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

    [HttpPost("hall/{attemptNumber:int}/next")]
    public IActionResult GetNextContenderForGivenAttempt(int attemptNumber, [FromQuery] int session)
    {
        try
        {
            if (_hall.GetQueueCount() == 0)
            {
                return Ok(
                    new ContenderFullNameDto
                    {
                        Name = null
                    });
            }
            var currentContender = (RatingContender)_hall.GetNextContender();
            _friend.NotifyAboutContender(currentContender);
            
            return Ok(
                new ContenderFullNameDto
                {
                    Name = currentContender.GetFullName()
                });
        }
        catch (Exception e)
        {
            return BadRequest(
                new ErrorDto
                {
                    Description = e.Message
                }
            );
        }
    }

    [HttpPost("hall/{attemptNumber:int}/select")]
    public IActionResult GetContenderRankFromAttempt(int attemptNumber, [FromQuery] int session)
    {
        try
        {
            if (_hall.GetLastContender() is RatingContender contender)
            {
                return Ok(
                    new ContenderRankDto
                    {
                        Rank = contender.Rating
                    });
            }

            return Ok(
                new ContenderRankDto
                {
                    Rank = null
                });
        }
        catch (Exception e)
        {
            return BadRequest(
                new ErrorDto
                {
                    Description = e.Message
                }
            );
        }
    }
}