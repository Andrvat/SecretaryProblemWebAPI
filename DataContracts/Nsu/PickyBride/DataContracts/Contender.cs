using DataContracts.Dtos;

namespace Nsu.PickyBride.DataContracts;

public interface Contender
{
    public ContenderFullNameDto? Name { get; set; }
}