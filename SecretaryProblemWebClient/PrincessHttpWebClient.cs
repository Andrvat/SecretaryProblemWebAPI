using System.Data;
using System.Text;
using DataContracts.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SecretaryProblemWebAPI;


namespace SecretaryProblemWebClient;

public class PrincessHttpWebClient
{
    private const string RemoteServerUri = "http://localhost:5197/";

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ContenderConsumerService _contenderConsumerService;
        
    public PrincessHttpWebClient(IServiceScopeFactory scopeFactory, ContenderConsumerService contenderConsumerService)
    {
        _scopeFactory = scopeFactory;
        _contenderConsumerService = contenderConsumerService;
    }

    public Contender GetNextContender()
    {
        using var scope = _scopeFactory.CreateScope();

        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;
        var response = GetConfiguredHttpClient().PostAsync(
            $"api/hall/{attemptId}/nextmq", new StringContent($"{attemptId}")).Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new DataException($"Remote server: {response.StatusCode}. An error occurred in the messages broker");
        }

        var contender = _contenderConsumerService.AwaitContender().Result;
        var contenderFullName = contender.Name?.Split(" ");
        return new Contender(
            surname: contenderFullName[0],
            name: contenderFullName[1],
            patronymic: contenderFullName[2]);
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
        var response = GetConfiguredHttpClient().PostAsync($"api/friend/{attemptId}/compare",
            new StringContent(
                JsonConvert.SerializeObject(contendersComparisonDto),
                Encoding.UTF8,
                "application/json")).Result;

        var bestContender = JsonConvert.DeserializeObject<ContenderFullNameDto>(GetResponseString(response));
        return firstContender.GetFullName() == bestContender.Name;
    }

    public int GetFinalContenderRank()
    {
        using var scope = _scopeFactory.CreateScope();
        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;

        var response = GetConfiguredHttpClient()
            .PostAsync($"api/hall/{attemptId}/select", new StringContent($"{attemptId}")).Result;

        var contenderRankDto = JsonConvert.DeserializeObject<ContenderRankDto>(GetResponseString(response));
        return contenderRankDto!.Rank.Value;
    }

    private static HttpClient GetConfiguredHttpClient()
    {
        var httpClient = new HttpClient(GetConfiguredSslClientHandler());
        httpClient.BaseAddress = new Uri(RemoteServerUri);
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
        if (!response.IsSuccessStatusCode)
        {
            var errorDto = JsonConvert.DeserializeObject<ErrorDto>(response.Content.ReadAsStringAsync().Result);
            throw new DataException($"Remote server: {response.StatusCode}. Description: {errorDto.Description}");
        }

        return response.Content.ReadAsStringAsync().Result;
    }
}