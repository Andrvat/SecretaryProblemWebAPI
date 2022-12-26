using DataContracts.Dtos;
using DataContracts.MassTransit;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace SecretaryProblemWebAPI.Controllers;

[ApiController]
[Route("api/hall/")]
public class HallController : ControllerBase
{
    private readonly ILogger<HallController> _logger;

    private readonly IPublishEndpoint _publishEndpoint;

    private readonly Hall _hall;
    private readonly Friend _friend;

    public HallController(ILogger<HallController> logger, Hall hall, Friend friend, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _hall = hall;
        _friend = friend;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("{attemptNumber:int}/next")]
    public async Task<IActionResult> GetNextContenderForGivenAttemptRabbitMq(int attemptNumber, [FromQuery] int session)
    {
        try
        {
            ContenderFullNameDto? dto;
            if (_hall.GetQueueCount() == 0)
            {
                dto = new ContenderFullNameDto { Name = null };
                await _publishEndpoint.Publish<IContenderCreated>(new { FullNameDto = dto });
                return Ok();
            }

            var currentContender = (RatingContender)_hall.GetNextContender();
            _friend.NotifyAboutContender(currentContender);

            dto = new ContenderFullNameDto { Name = currentContender.GetFullName() };
            await _publishEndpoint.Publish<IContenderCreated>(new { FullNameDto = dto });
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPost("{attemptNumber:int}/select")]
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