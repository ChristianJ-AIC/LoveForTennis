using LoveForTennis.Application.DTOs;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Interfaces;

namespace LoveForTennis.Application.Services;

public class BookingPlayerService : IBookingPlayerService
{
    private readonly IBookingPlayerRepository _bookingPlayerRepository;

    public BookingPlayerService(IBookingPlayerRepository bookingPlayerRepository)
    {
        _bookingPlayerRepository = bookingPlayerRepository;
    }

    public async Task<IEnumerable<BookingPlayerDto>> GetAllBookingPlayersAsync()
    {
        var entities = await _bookingPlayerRepository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<BookingPlayerDto?> GetBookingPlayerByIdAsync(int id)
    {
        var entity = await _bookingPlayerRepository.GetByIdAsync(id);
        if (entity == null) return null;

        return MapToDto(entity);
    }

    public async Task<IEnumerable<BookingPlayerDto>> GetBookingPlayersByBookingIdAsync(int bookingId)
    {
        var entities = await _bookingPlayerRepository.GetByBookingIdAsync(bookingId);
        return entities.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingPlayerDto>> GetBookingPlayersByPlayerUserIdAsync(string playerUserId)
    {
        var entities = await _bookingPlayerRepository.GetByPlayerUserIdAsync(playerUserId);
        return entities.Select(MapToDto);
    }

    public async Task<BookingPlayerDto> CreateBookingPlayerAsync(BookingPlayerDto bookingPlayerDto)
    {
        var entity = new BookingPlayer
        {
            BookingId = bookingPlayerDto.BookingId,
            PlayerUserId = bookingPlayerDto.PlayerUserId
        };

        var createdEntity = await _bookingPlayerRepository.CreateAsync(entity);
        return MapToDto(createdEntity);
    }

    public async Task<BookingPlayerDto> UpdateBookingPlayerAsync(BookingPlayerDto bookingPlayerDto)
    {
        var entity = new BookingPlayer
        {
            Id = bookingPlayerDto.Id,
            BookingId = bookingPlayerDto.BookingId,
            PlayerUserId = bookingPlayerDto.PlayerUserId,
            Created = bookingPlayerDto.Created
        };

        var updatedEntity = await _bookingPlayerRepository.UpdateAsync(entity);
        return MapToDto(updatedEntity);
    }

    public async Task DeleteBookingPlayerAsync(int id)
    {
        await _bookingPlayerRepository.DeleteAsync(id);
    }

    private static BookingPlayerDto MapToDto(BookingPlayer entity)
    {
        return new BookingPlayerDto
        {
            Id = entity.Id,
            BookingId = entity.BookingId,
            PlayerUserId = entity.PlayerUserId,
            Created = entity.Created,
            LastUpdated = entity.LastUpdated,
            PlayerUserName = entity.PlayerUser?.UserName ?? string.Empty
        };
    }
}