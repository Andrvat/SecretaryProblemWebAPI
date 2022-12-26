using DataContracts.Common;

namespace SecretaryProblemWebAPI.Generators;

public interface IGenerator
{
    public List<Contender>? GetContenders();
    
    public void CreateContenders();
}