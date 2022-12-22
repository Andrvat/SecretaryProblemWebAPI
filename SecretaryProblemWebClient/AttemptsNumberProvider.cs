namespace SecretaryProblemWebClient;

public class AttemptsNumberProvider
{
    public int? AttemptNumber { get; set; }

    public AttemptsNumberProvider(CliArgumentsParser argsProvider)
    {
        AttemptNumber = argsProvider.AttemptNumber;
    }
}