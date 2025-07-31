using Microsoft.EntityFrameworkCore;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Interfaces;
using LoveForTennis.Infrastructure.Data;

namespace LoveForTennis.Infrastructure.Repositories;

public class DummyRepository : IDummyRepository
{
    private readonly ApplicationDbContext _context;

    public DummyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DummyEntity>> GetAllAsync()
    {
        return await _context.DummyEntities.ToListAsync();
    }

    public async Task<DummyEntity?> GetByIdAsync(int id)
    {
        return await _context.DummyEntities.FindAsync(id);
    }

    public async Task<DummyEntity> CreateAsync(DummyEntity entity)
    {
        _context.DummyEntities.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<DummyEntity> UpdateAsync(DummyEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.DummyEntities.FindAsync(id);
        if (entity != null)
        {
            _context.DummyEntities.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}