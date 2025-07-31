using LoveForTennis.Application.DTOs;

namespace LoveForTennis.Application.Interfaces;

public interface IDummyService
{
    Task<IEnumerable<DummyDto>> GetAllDummiesAsync();
    Task<DummyDto?> GetDummyByIdAsync(int id);
    Task<DummyDto> CreateDummyAsync(DummyDto dummyDto);
    Task<DummyDto> UpdateDummyAsync(DummyDto dummyDto);
    Task DeleteDummyAsync(int id);
}