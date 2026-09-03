using EventParkingReservation.DTOs.Event;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Implementations;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly ICategoryRepository _categoryRepository;

        public EventService(
            IEventRepository eventRepository,
            IVenueRepository venueRepository,
            ICategoryRepository categoryRepository)
        {
            _eventRepository = eventRepository;
            _venueRepository = venueRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync(
            string? search,
            int? venueId,
            int? categoryId,
            DateOnly? date)
        {
            var events = await _eventRepository.GetAllAsync(
                search,
                venueId,
                categoryId,
                date);

            return events.Select(Map);
        }

        public async Task<EventDto> GetByIdAsync(int id)
        {
            var eventEntity =
                await _eventRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            return Map(eventEntity);
        }

        public async Task<EventDto> CreateAsync(
            CreateEventDto dto)
        {
            var venue =
                await _venueRepository.GetByIdAsync(
                    dto.VenueId)
                ?? throw new KeyNotFoundException(
                    "Venue not found.");

            var category =
                await _categoryRepository.GetByIdAsync(
                    dto.CategoryId)
                ?? throw new KeyNotFoundException(
                    "Category not found.");

            if (dto.EventDate <=
                DateOnly.FromDateTime(DateTime.Today))
            {
                throw new InvalidOperationException(
                    "Event date must be in the future.");
            }

            if (dto.StartTime >= dto.EndTime)
            {
                throw new InvalidOperationException(
                    "End time must be after start time.");
            }

            if (dto.Capacity > venue.Capacity)
            {
                throw new InvalidOperationException(
                    "Event capacity cannot exceed venue capacity.");
            }

            var eventEntity = new Event
            {
                EventName = dto.EventName.Trim(),
                EventDate = dto.EventDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Description = dto.Description,
                TicketPrice = dto.TicketPrice,
                ParkingFee = dto.ParkingFee,
                Capacity = dto.Capacity,
                VenueId = dto.VenueId,
                CategoryId = dto.CategoryId
            };

            await _eventRepository.AddAsync(
                eventEntity);

            eventEntity.Venue = venue;
            eventEntity.Category = category;

            return Map(eventEntity);
        }

        public async Task<EventDto> UpdateAsync(
            int id,
            UpdateEventDto dto)
        {
            var eventEntity =
                await _eventRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            var venue =
                await _venueRepository.GetByIdAsync(
                    dto.VenueId)
                ?? throw new KeyNotFoundException(
                    "Venue not found.");

            var category =
                await _categoryRepository.GetByIdAsync(
                    dto.CategoryId)
                ?? throw new KeyNotFoundException(
                    "Category not found.");

            bool hasBookings =
                await _eventRepository
                    .HasActiveBookingsAsync(id);

            if (hasBookings &&
                eventEntity.TicketPrice !=
                dto.TicketPrice)
            {
                throw new InvalidOperationException(
                    "Ticket price cannot be changed after bookings exist.");
            }

            if (dto.Capacity > venue.Capacity)
            {
                throw new InvalidOperationException(
                    "Event capacity cannot exceed venue capacity.");
            }

            eventEntity.EventName =
                dto.EventName.Trim();

            eventEntity.EventDate =
                dto.EventDate;

            eventEntity.StartTime =
                dto.StartTime;

            eventEntity.EndTime =
                dto.EndTime;

            eventEntity.Description =
                dto.Description;

            eventEntity.TicketPrice =
                dto.TicketPrice;

            eventEntity.ParkingFee =
                dto.ParkingFee;

            eventEntity.Capacity =
                dto.Capacity;

            eventEntity.VenueId =
                dto.VenueId;

            eventEntity.CategoryId =
                dto.CategoryId;

            await _eventRepository.UpdateAsync(
                eventEntity);

            eventEntity.Venue = venue;
            eventEntity.Category = category;

            return Map(eventEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var eventEntity =
                await _eventRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            if (await _eventRepository
                .HasActiveBookingsAsync(id))
            {
                throw new InvalidOperationException(
                    "Event cannot be deleted because active bookings exist.");
            }

            await _eventRepository.DeleteAsync(
                eventEntity);
        }

        private static EventDto Map(
            Event eventEntity)
        {
            return new EventDto
            {
                EventId =
                    eventEntity.EventId,

                EventName =
                    eventEntity.EventName,

                EventDate =
                    eventEntity.EventDate,

                StartTime =
                    eventEntity.StartTime,

                EndTime =
                    eventEntity.EndTime,

                Description =
                    eventEntity.Description,

                TicketPrice =
                    eventEntity.TicketPrice,

                ParkingFee =
                    eventEntity.ParkingFee,

                Capacity =
                    eventEntity.Capacity,

                VenueId =
                    eventEntity.VenueId,

                VenueName =
                    eventEntity.Venue?.VenueName
                    ?? string.Empty,

                CategoryId =
                    eventEntity.CategoryId,

                CategoryName =
                    eventEntity.Category
                        ?.CategoryName
                    ?? string.Empty
            };
        }
    }
}
