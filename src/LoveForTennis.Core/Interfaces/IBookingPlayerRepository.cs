using LoveForTennis.Core.Entities;

namespace LoveForTennis.Core.Interfaces;

public interface IBookingPlayerRepository
{
    Task<IEnumerable<BookingPlayer>> GetAllAsync();
    Task<BookingPlayer?> GetByIdAsync(int id);
    Task<IEnumerable<BookingPlayer>> GetByBookingIdAsync(int bookingId);
    Task<IEnumerable<BookingPlayer>> GetByPlayerUserIdAsync(string playerUserId);
    Task<BookingPlayer> CreateAsync(BookingPlayer entity);
    Task<BookingPlayer> UpdateAsync(BookingPlayer entity);
    Task DeleteAsync(int id);
}