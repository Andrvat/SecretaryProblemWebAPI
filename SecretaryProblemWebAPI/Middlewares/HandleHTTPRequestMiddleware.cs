using System.Text;
using SecretaryProblemWebAPI.Generators;

namespace SecretaryProblemWebAPI.Middlewares;

public class HandleHttpRequestMiddleware
{
    private readonly RequestDelegate _next;

    private readonly AttemptsDbConfigurator _attemptsDbConfigurator;
    private readonly ContendersDbGenerator _contendersDbGenerator;

    private readonly AttemptsNumberProvider _attemptsNumberProvider;

    private readonly Hall _hall;

    public HandleHttpRequestMiddleware(RequestDelegate next,
        AttemptsDbConfigurator attemptsDbConfigurator,
        AttemptsNumberProvider attemptsNumberProvider,
        ContendersDbGenerator contendersDbGenerator,
        Hall hall)
    {
        _next = next;
        _attemptsDbConfigurator = attemptsDbConfigurator;
        _attemptsNumberProvider = attemptsNumberProvider;
        _contendersDbGenerator = contendersDbGenerator;
        _hall = hall;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await HandleRequest(context);
            await _next(context);
        }
        catch (Exception e)
        {
            var bytes = Encoding.UTF8.GetBytes(e.Message);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }

    private Task HandleRequest(HttpContext context)
    {
        var uriPath = context.Request.Path.Value;
        var uriTokens = uriPath?.Split("/");

        if (uriTokens.Length < 3)
        {
            throw new UriFormatException("Bad request");
        }

        if (int.TryParse(uriTokens?[2], out var requestAttemptNumber))
        {
            if (requestAttemptNumber is < 0 or > AttemptsDbConfigurator.AttemptsNumber)
            {
                throw new UriFormatException("Invalid attempt number in HTTP request");
            }

            if (requestAttemptNumber == _attemptsNumberProvider.AttemptNumber)
            {
                return Task.CompletedTask;
            }

            switch (uriTokens[1].ToLowerInvariant())
            {
                case "hall":
                {
                    _attemptsNumberProvider.AttemptNumber = requestAttemptNumber;
                    _attemptsDbConfigurator.ConfigureAttempts();
                    _contendersDbGenerator.CreateContenders();
                    _hall.InviteContenders(_contendersDbGenerator.GetContenders());
                    _hall.NotifyFriendAboutReset();
                    break;
                }
                default:
                    throw new UriFormatException("Another attempt number is available only in the new request to hall");
            }
        }
        else
        {
            throw new UriFormatException("Attempt should be a positive integer");
        }

        return Task.CompletedTask;
    }
}