using System.Data;
using System.Text;
using DataContracts.Common;
using DataContracts.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SecretaryProblemWebAPI;


namespace SecretaryProblemWebClient;

public class PrincessHttpWebClient
{
    private const string RemoteServerUri = "http://localhost:5197/";

    private readonly IServiceScopeFactory _scopeFactory;

    public PrincessHttpWebClient(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Contender GetNextContender()
    {
        using var scope = _scopeFactory.CreateScope();

        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;
        var response = GetConfiguredHttpClient().PostAsync(
            $"hall/{attemptId}/next", new StringContent($"{attemptId}")).Result;

        // TODO: throw exception
        if (!response.IsSuccessStatusCode) return null;

        var responseString = response.Content.ReadAsStringAsync().Result;
        var contender = JsonConvert.DeserializeObject<ContenderFullNameDto>(responseString);
        var contenderFullName = contender.Name.Split(" ");
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
        var response = GetConfiguredHttpClient().PostAsync($"friend/{attemptId}/compare",
            new StringContent(
                JsonConvert.SerializeObject(contendersComparisonDto),
                Encoding.UTF8,
                "application/json")).Result;
        
        if (!response.IsSuccessStatusCode)
        {
            throw new DataException("");
        };

        var responseString = response.Content.ReadAsStringAsync().Result;
        var bestContender = JsonConvert.DeserializeObject<ContenderFullNameDto>(responseString);
        return firstContender.GetFullName() == bestContender.Name;
    }

    public int GetFinalContenderRank()
    {
        using var scope = _scopeFactory.CreateScope();
        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;

        var response = GetConfiguredHttpClient().PostAsync($"hall/{attemptId}/select", new StringContent($"{attemptId}")).Result;

        // TODO: throw exception
        if (!response.IsSuccessStatusCode) return 0;

        var responseString = response.Content.ReadAsStringAsync().Result;
        var contenderRankDto = JsonConvert.DeserializeObject<ContenderRankDto>(responseString);
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
}