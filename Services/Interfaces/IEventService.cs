using EventParkingReservation.DTOs.Event;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync(
            string? search,
            int? venueId,
            int? categoryId,
            DateOnly? date);

        Task<EventDto> GetByIdAsync(int id);

        Task<EventDto> CreateAsync(
            CreateEventDto dto);

        Task<EventDto> UpdateAsync(
            int id,
            UpdateEventDto dto);

        Task DeleteAsync(int id);
    }
}
