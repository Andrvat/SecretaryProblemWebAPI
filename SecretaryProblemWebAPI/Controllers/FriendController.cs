using DataContracts.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace SecretaryProblemWebAPI.Controllers;

[ApiController]
[Route("api/friend/")]
public class FriendController : ControllerBase
{
    private readonly Friend _friend;

    public FriendController(Friend friend)
    {
        _friend = friend;
    }

    [HttpPost("{attemptId:int}/compare")]
    public IActionResult CompareContenders(int attemptId,
        [FromBody] ContendersComparisonDto contendersComparisonDto,
        [FromQuery] string sessionId)
    {
        try
        {
            return Ok(
                _friend.ReplyToComparison(
                    contendersComparisonDto.Name1,
                    contendersComparisonDto.Name2)
                    ? new ContenderFullNameDto
                    {
                        Name = contendersComparisonDto.Name1
                    }
                    : new ContenderFullNameDto
                    {
                        Name = contendersComparisonDto.Name2
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