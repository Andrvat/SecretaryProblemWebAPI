using System.Data;
using System.Diagnostics;
using System.Text;
using DataContracts.Common;
using DataContracts.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace SecretaryProblemWebClient;

public class PrincessHttpWebClient
{
    private readonly string _remoteServerUri;
    private readonly string _remoteServerSession;

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ContenderConsumerService _contenderConsumerService;

    public PrincessHttpWebClient(IServiceScopeFactory scopeFactory, ContenderConsumerService contenderConsumerService)
    {
        _scopeFactory = scopeFactory;
        _contenderConsumerService = contenderConsumerService;
        _remoteServerUri = System.Configuration.ConfigurationManager.AppSettings["remote-server-uri-competition"]!;
        _remoteServerSession = System.Configuration.ConfigurationManager.AppSettings["session-competition"]!;
    }

    public Contender? GetNextContender()
    {
        using var scope = _scopeFactory.CreateScope();

        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;
        var response = GetConfiguredHttpClient().PostAsync(
            $"api/hall/{attemptId}/next?sessionId={_remoteServerSession}", new StringContent($"{attemptId}")).Result;

        if (!response.IsSuccessStatusCode)
        {
            var error = response.Content.ReadAsStringAsync().Result;
            throw new DataException($"Remote server: {response.StatusCode}. Raw description: {error}");
        }

        var contenderName = _contenderConsumerService.AwaitContender().Result;
        if (contenderName == "ABC")
        {
            return null;
        }
        var contenderFullName = contenderName.Split(" ");
        return new Contender(
            surname: contenderFullName[0],
            name: contenderFullName[1]);
    }

    public bool CompareContenders(Contender firstContender, Contender secondContender)
    {
        using var scope = _scopeFactory.CreateScope();

        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;
        var contendersComparisonDto = new ContendersComparisonDto
        {
            Name1 = firstContender.GetFullName(),
            Name2 = secondContender.GetFullName()
        };
        var response = GetConfiguredHttpClient().PostAsync(
            $"api/freind/{attemptId}/compare?sessionId={_remoteServerSession}",
            new StringContent(
                JsonConvert.SerializeObject(contendersComparisonDto),
                Encoding.UTF8,
                "application/json")).Result;

        var bestContender = GetResponseString(response);
        return firstContender.GetFullName() == bestContender;
    }

    public int GetFinalContenderRank()
    {
        using var scope = _scopeFactory.CreateScope();
        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;

        var response = GetConfiguredHttpClient()
            .PostAsync($"api/hall/{attemptId}/select?sessionId={_remoteServerSession}",
                new StringContent($"{attemptId}")).Result;

        var responseString = GetResponseString(response);
        return int.Parse(responseString);
    }

    private HttpClient GetConfiguredHttpClient()
    {
        var httpClient = new HttpClient(GetConfiguredSslClientHandler());
        httpClient.BaseAddress = new Uri(_remoteServerUri);
        httpClient.Timeout = TimeSpan.FromMinutes(5);
        return httpClient;
    }

    private static HttpClientHandler GetConfiguredSslClientHandler()
    {
        var clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        return clientHandler;
    }

    private static string GetResponseString(HttpResponseMessage? response)
    {
        if (response.IsSuccessStatusCode)
        {
            return response.Content.ReadAsStringAsync().Result;
        }
        var error = response.Content.ReadAsStringAsync().Result;
        throw new DataException($"Remote server: {response.StatusCode}. Raw description: {error}");

    }
}