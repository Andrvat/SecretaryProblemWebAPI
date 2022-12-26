namespace DataContracts.Common;

public class Contender
{
    public string Surname { get; init; }
    public string Name { get; init; }

    public Contender(string surname, string name)
    {
        Surname = surname ?? throw new ArgumentNullException(nameof(surname));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public string GetFullName()
    {
        return $"{Surname} {Name}";
    }
}