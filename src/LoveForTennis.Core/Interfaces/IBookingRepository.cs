using LoveForTennis.Core.Entities;

namespace LoveForTennis.Core.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(int id);
    Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
    Task<Booking> CreateAsync(Booking entity);
    Task<Booking> UpdateAsync(Booking entity);
    Task DeleteAsync(int id);
}