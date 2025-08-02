using Microsoft.EntityFrameworkCore;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Interfaces;
using LoveForTennis.Infrastructure.Data;

namespace LoveForTennis.Infrastructure.Repositories;

public class BookingPlayerRepository : IBookingPlayerRepository
{
    private readonly ApplicationDbContext _context;

    public BookingPlayerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingPlayer>> GetAllAsync()
    {
        return await _context.BookingPlayers
            .Include(bp => bp.Booking)
            .Include(bp => bp.PlayerUser)
            .ToListAsync();
    }

    public async Task<BookingPlayer?> GetByIdAsync(int id)
    {
        return await _context.BookingPlayers
            .Include(bp => bp.Booking)
            .Include(bp => bp.PlayerUser)
            .FirstOrDefaultAsync(bp => bp.Id == id);
    }

    public async Task<IEnumerable<BookingPlayer>> GetByBookingIdAsync(int bookingId)
    {
        return await _context.BookingPlayers
            .Include(bp => bp.Booking)
            .Include(bp => bp.PlayerUser)
            .Where(bp => bp.BookingId == bookingId)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingPlayer>> GetByPlayerUserIdAsync(string playerUserId)
    {
        return await _context.BookingPlayers
            .Include(bp => bp.Booking)
            .Include(bp => bp.PlayerUser)
            .Where(bp => bp.PlayerUserId == playerUserId)
            .ToListAsync();
    }

    public async Task<BookingPlayer> CreateAsync(BookingPlayer entity)
    {
        entity.Created = DateTime.UtcNow;
        _context.BookingPlayers.Add(entity);
        await _context.SaveChangesAsync();
        
        // Return entity without trying to load non-existent navigation properties
        return entity;
    }

    public async Task<BookingPlayer> UpdateAsync(BookingPlayer entity)
    {
        entity.LastUpdated = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.BookingPlayers.FindAsync(id);
        if (entity != null)
        {
            _context.BookingPlayers.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}