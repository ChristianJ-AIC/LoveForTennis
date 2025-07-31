using LoveForTennis.Application.DTOs;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Interfaces;

namespace LoveForTennis.Application.Services;

public class DummyService : IDummyService
{
    private readonly IDummyRepository _dummyRepository;

    public DummyService(IDummyRepository dummyRepository)
    {
        _dummyRepository = dummyRepository;
    }

    public async Task<IEnumerable<DummyDto>> GetAllDummiesAsync()
    {
        var entities = await _dummyRepository.GetAllAsync();
        return entities.Select(e => new DummyDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            CreatedAt = e.CreatedAt
        });
    }

    public async Task<DummyDto?> GetDummyByIdAsync(int id)
    {
        var entity = await _dummyRepository.GetByIdAsync(id);
        if (entity == null) return null;

        return new DummyDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<DummyDto> CreateDummyAsync(DummyDto dummyDto)
    {
        var entity = new DummyEntity
        {
            Name = dummyDto.Name,
            Description = dummyDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        var createdEntity = await _dummyRepository.CreateAsync(entity);

        return new DummyDto
        {
            Id = createdEntity.Id,
            Name = createdEntity.Name,
            Description = createdEntity.Description,
            CreatedAt = createdEntity.CreatedAt
        };
    }

    public async Task<DummyDto> UpdateDummyAsync(DummyDto dummyDto)
    {
        var entity = new DummyEntity
        {
            Id = dummyDto.Id,
            Name = dummyDto.Name,
            Description = dummyDto.Description,
            CreatedAt = dummyDto.CreatedAt
        };

        var updatedEntity = await _dummyRepository.UpdateAsync(entity);

        return new DummyDto
        {
            Id = updatedEntity.Id,
            Name = updatedEntity.Name,
            Description = updatedEntity.Description,
            CreatedAt = updatedEntity.CreatedAt
        };
    }

    public async Task DeleteDummyAsync(int id)
    {
        await _dummyRepository.DeleteAsync(id);
    }
}