using Microsoft.EntityFrameworkCore;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Interfaces;
using LoveForTennis.Infrastructure.Data;

namespace LoveForTennis.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.BookedByUser)
            .Include(b => b.Court)
            .Include(b => b.Players)
                .ThenInclude(bp => bp.PlayerUser)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.BookedByUser)
            .Include(b => b.Court)
            .Include(b => b.Players)
                .ThenInclude(bp => bp.PlayerUser)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
    {
        return await _context.Bookings
            .Include(b => b.BookedByUser)
            .Include(b => b.Court)
            .Include(b => b.Players)
                .ThenInclude(bp => bp.PlayerUser)
            .Where(b => b.BookedByUserId == userId)
            .ToListAsync();
    }

    public async Task<Booking> CreateAsync(Booking entity)
    {
        entity.Created = DateTime.UtcNow;
        _context.Bookings.Add(entity);
        await _context.SaveChangesAsync();
        
        // Return entity without trying to load non-existent navigation properties
        return entity;
    }

    public async Task<Booking> UpdateAsync(Booking entity)
    {
        entity.LastUpdated = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Bookings.FindAsync(id);
        if (entity != null)
        {
            _context.Bookings.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}