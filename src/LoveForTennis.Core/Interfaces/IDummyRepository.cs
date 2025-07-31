using LoveForTennis.Core.Entities;

namespace LoveForTennis.Core.Interfaces;

public interface IDummyRepository
{
    Task<IEnumerable<DummyEntity>> GetAllAsync();
    Task<DummyEntity?> GetByIdAsync(int id);
    Task<DummyEntity> CreateAsync(DummyEntity entity);
    Task<DummyEntity> UpdateAsync(DummyEntity entity);
    Task DeleteAsync(int id);
}