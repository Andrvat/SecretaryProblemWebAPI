using System.Data;
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

    public async Task<Contender> GetNextContender()
    {
        using var scope = _scopeFactory.CreateScope();

        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;
        Console.WriteLine("Web client will do POST request");
        await Console.Out.FlushAsync();
        var response = await GetConfiguredHttpClient().PostAsync(
            $"api/hall/{attemptId}/next?sessionId={_remoteServerSession}", new StringContent($"{attemptId}"));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new DataException($"Remote server: {response.StatusCode}. Raw description: {error}");
        }
        
        var contenderName = await _contenderConsumerService.AwaitContender();
        var contenderFullName = contenderName.Split(" ");
        Console.WriteLine($"Web client notifies about new name: {contenderFullName[0]} {contenderFullName[1]}");
        await Console.Out.FlushAsync();
        return new Contender(
            surname: contenderFullName[0],
            name: contenderFullName[1]);
    }

    public async Task<bool> CompareContenders(Contender firstContender, Contender secondContender)
    {
        using var scope = _scopeFactory.CreateScope();

        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;
        var contendersComparisonDto = new ContendersComparisonDto
        {
            Name1 = firstContender.GetFullName(),
            Name2 = secondContender.GetFullName()
        };
        var response = await GetConfiguredHttpClient().PostAsync(
            $"api/freind/{attemptId}/compare?sessionId={_remoteServerSession}",
            new StringContent(
                JsonConvert.SerializeObject(contendersComparisonDto),
                Encoding.UTF8,
                "application/json"));

        var bestContender = JsonConvert.DeserializeObject<ContenderFullNameDto>(await GetResponseString(response));
        return firstContender.GetFullName() == bestContender.Name;
    }

    public async Task<int> GetFinalContenderRank()
    {
        using var scope = _scopeFactory.CreateScope();
        var attemptId = scope.ServiceProvider.GetService<AttemptsNumberProvider>()!.AttemptNumber;

        var response = await GetConfiguredHttpClient()
            .PostAsync($"api/hall/{attemptId}/select?sessionId={_remoteServerSession}",
                new StringContent($"{attemptId}"));

        var contenderRankDto = JsonConvert.DeserializeObject<ContenderRankDto>(await GetResponseString(response));
        return contenderRankDto!.Rank.Value;
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

    private static async Task<string> GetResponseString(HttpResponseMessage? response)
    {
        // if (!response.IsSuccessStatusCode)
        // {
        //     var errorDto = JsonConvert.DeserializeObject<ErrorDto>(response.Content.ReadAsStringAsync().Result);
        //     throw new DataException($"Remote server: {response.StatusCode}. Description: {errorDto.Description}");
        // }
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new DataException($"Remote server: {response.StatusCode}. Raw description: {error}");
        }

        return  await response.Content.ReadAsStringAsync();
    }
}