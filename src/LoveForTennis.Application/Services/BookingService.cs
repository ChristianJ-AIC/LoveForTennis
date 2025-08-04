using LoveForTennis.Application.DTOs;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Interfaces;

namespace LoveForTennis.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
    {
        var entities = await _bookingRepository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int id)
    {
        var entity = await _bookingRepository.GetByIdAsync(id);
        if (entity == null) return null;

        return MapToDto(entity);
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(string userId)
    {
        var entities = await _bookingRepository.GetByUserIdAsync(userId);
        return entities.Select(MapToDto);
    }

    public async Task<BookingDto> CreateBookingAsync(BookingDto bookingDto)
    {
        var entity = new Booking
        {
            BookedByUserId = bookingDto.BookedByUserId,
            CourtId = bookingDto.CourtId,
            BookingFrom = bookingDto.BookingFrom,
            BookingTo = bookingDto.BookingTo,
            Cancelled = bookingDto.Cancelled,
            BookingType = bookingDto.BookingType
        };

        var createdEntity = await _bookingRepository.CreateAsync(entity);
        return MapToDto(createdEntity);
    }

    public async Task<BookingDto> UpdateBookingAsync(BookingDto bookingDto)
    {
        var entity = new Booking
        {
            Id = bookingDto.Id,
            BookedByUserId = bookingDto.BookedByUserId,
            CourtId = bookingDto.CourtId,
            BookingFrom = bookingDto.BookingFrom,
            BookingTo = bookingDto.BookingTo,
            Cancelled = bookingDto.Cancelled,
            Created = bookingDto.Created,
            BookingType = bookingDto.BookingType
        };

        var updatedEntity = await _bookingRepository.UpdateAsync(entity);
        return MapToDto(updatedEntity);
    }

    public async Task DeleteBookingAsync(int id)
    {
        await _bookingRepository.DeleteAsync(id);
    }

    private static BookingDto MapToDto(Booking entity)
    {
        return new BookingDto
        {
            Id = entity.Id,
            BookedByUserId = entity.BookedByUserId,
            CourtId = entity.CourtId,
            BookingFrom = entity.BookingFrom,
            BookingTo = entity.BookingTo,
            Cancelled = entity.Cancelled,
            Created = entity.Created,
            LastUpdated = entity.LastUpdated,
            BookingType = entity.BookingType,
            BookedByUserName = entity.BookedByUser?.UserName ?? string.Empty,
            CourtName = entity.Court?.Name ?? string.Empty,
            Players = entity.Players?.Select(bp => new BookingPlayerDto
            {
                Id = bp.Id,
                BookingId = bp.BookingId,
                PlayerUserId = bp.PlayerUserId,
                Created = bp.Created,
                LastUpdated = bp.LastUpdated,
                PlayerUserName = bp.PlayerUser?.UserName ?? string.Empty
            }).ToList() ?? new List<BookingPlayerDto>()
        };
    }
}