namespace SecretaryProblemWebAPI;

public class Contender
{
    public string Surname { get; }
    public string Name { get; }
    public string Patronymic { get; }

    protected Contender(string surname, string name, string patronymic)
    {
        Surname = surname ?? throw new ArgumentNullException(nameof(surname));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Patronymic = patronymic ?? throw new ArgumentNullException(nameof(patronymic));
    }

    public string GetFullName()
    {
        return $"{Surname} {Name} {Patronymic}";
    }
}