namespace SecretaryProblemWebClient;

public class CliArgumentsParser
{ 
    public int? AttemptNumber { get; set; }

    public CliArgumentsParser(string[] args)
    {
        AttemptNumber = args.Length switch
        {
            0 => null,
            1 => int.Parse(args[0]),
            _ => throw new ArgumentException("Invalid argument number")
        };
    }
}