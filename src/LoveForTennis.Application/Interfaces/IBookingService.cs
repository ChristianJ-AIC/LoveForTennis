using LoveForTennis.Application.DTOs;

namespace LoveForTennis.Application.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
    Task<BookingDto?> GetBookingByIdAsync(int id);
    Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(string userId);
    Task<BookingDto> CreateBookingAsync(BookingDto bookingDto);
    Task<BookingDto> UpdateBookingAsync(BookingDto bookingDto);
    Task DeleteBookingAsync(int id);
}