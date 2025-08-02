using LoveForTennis.Application.DTOs;

namespace LoveForTennis.Application.Interfaces;

public interface IBookingPlayerService
{
    Task<IEnumerable<BookingPlayerDto>> GetAllBookingPlayersAsync();
    Task<BookingPlayerDto?> GetBookingPlayerByIdAsync(int id);
    Task<IEnumerable<BookingPlayerDto>> GetBookingPlayersByBookingIdAsync(int bookingId);
    Task<IEnumerable<BookingPlayerDto>> GetBookingPlayersByPlayerUserIdAsync(string playerUserId);
    Task<BookingPlayerDto> CreateBookingPlayerAsync(BookingPlayerDto bookingPlayerDto);
    Task<BookingPlayerDto> UpdateBookingPlayerAsync(BookingPlayerDto bookingPlayerDto);
    Task DeleteBookingPlayerAsync(int id);
}