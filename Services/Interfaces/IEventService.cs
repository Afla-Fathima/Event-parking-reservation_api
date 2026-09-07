using EventParkingReservation.DTOs.Event;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllAsync(
            string? search,
            DateOnly? date,
            int? venueId,
            int? categoryId);

        Task<EventDto?> GetByIdAsync(
            int id);

        Task<EventDto> CreateAsync(
            CreateEventDto dto);

        Task<EventDto> UpdateAsync(
            int id,
            UpdateEventDto dto);

        Task DeleteAsync(
            int id);
    }
}