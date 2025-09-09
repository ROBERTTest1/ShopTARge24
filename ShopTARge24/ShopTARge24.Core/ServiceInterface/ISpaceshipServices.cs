using ShopTARge24.Core.Domain;
using ShopTARge24.Core.Dto;


namespace ShopTARge24.Core.ServiceInterface
{
    public interface ISpaceshipServices
    {
        Task<Spaceships> Create(SpaceshipDto dto);
        Task<Spaceships> DetailAsync(Guid id);
    }
}
