using EventParkingReservation.DTOs.Event;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repo;
        private readonly INotificationService
            _notifications;

        public EventService(
            IEventRepository repo,
            INotificationService notifications)
        {
            _repo = repo;
            _notifications = notifications;
        }

        public async Task<List<EventDto>>
            GetAllAsync(
                string? search,
                DateOnly? date,
                int? venueId,
                int? categoryId)
        {
            var events =
                await _repo.GetAllAsync(
                    search,
                    date,
                    venueId,
                    categoryId);

            return events
                .Select(Map)
                .ToList();
        }

        public async Task<EventDto?>
            GetByIdAsync(
                int id)
        {
            var entity =
                await _repo.GetByIdAsync(id);

            return entity == null
                ? null
                : Map(entity);
        }

        public async Task<EventDto>
            CreateAsync(
                CreateEventDto dto)
        {
            await ValidateAsync(
                dto,
                null);

            var entity =
                new EventParkingReservation.Models.Event
                {
                    EventName =
                        dto.EventName.Trim(),

                    EventDate =
                        dto.EventDate,

                    StartTime =
                        dto.StartTime,

                    EndTime =
                        dto.EndTime,

                    Description =
                        dto.Description.Trim(),

                    TicketPrice =
                        dto.TicketPrice,

                    ParkingFee =
                        dto.ParkingFee,

                    Capacity =
                        dto.Capacity,

                    VenueId =
                        dto.VenueId,

                    CategoryId =
                        dto.CategoryId
                };

            await _repo.AddAsync(entity);

            return (await GetByIdAsync(
                entity.EventId))!;
        }

        public async Task<EventDto>
            UpdateAsync(
                int id,
                UpdateEventDto dto)
        {
            var entity =
                await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            await ValidateAsync(
                dto,
                id);

            bool hasAnyBookings =
          await _repo
         .HasAnyBookingsAsync(
             id);

            if (
                hasAnyBookings &&
                dto.TicketPrice !=
                    entity.TicketPrice
            )
            {
                throw new InvalidOperationException(
                    "Ticket price cannot be changed after bookings exist.");
            }

            int bookedSeatCount =
                await _repo
                    .BookedSeatCountAsync(id);

            if (dto.Capacity <
                bookedSeatCount)
            {
                throw new InvalidOperationException(
                    "Capacity cannot be lower than the number of booked seats.");
            }

            var customerIds =
                await _repo
                    .GetActiveBookingCustomerIdsAsync(
                        id);

            entity.EventName =
                dto.EventName.Trim();

            entity.EventDate =
                dto.EventDate;

            entity.StartTime =
                dto.StartTime;

            entity.EndTime =
                dto.EndTime;

            entity.Description =
                dto.Description.Trim();

            entity.TicketPrice =
                dto.TicketPrice;

            entity.ParkingFee =
                dto.ParkingFee;

            entity.Capacity =
                dto.Capacity;

            entity.VenueId =
                dto.VenueId;

            entity.CategoryId =
                dto.CategoryId;

            await _repo.UpdateAsync(
                entity);

            var updated =
                (await GetByIdAsync(id))!;

            foreach (int customerId
                     in customerIds)
            {
                await _notifications
                    .CreateAsync(
                        customerId,
                        "Event Updated",
                        $"{updated.EventName} has been updated. Date: {updated.EventDate:yyyy-MM-dd}, Time: {updated.StartTime:HH\\:mm}.",
                        "EventUpdate");
            }

            return updated;
        }

        public async Task DeleteAsync(
            int id)
        {
            var entity =
                await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            if (await _repo
                .HasActiveBookingsAsync(id))
            {
                throw new InvalidOperationException(
                    "Event has active bookings and cannot be deleted.");
            }

            await _repo.DeleteAsync(entity);
        }

        private async Task ValidateAsync(
            CreateEventDto dto,
            int? excludeEventId)
        {
            if (dto.EndTime <=
                dto.StartTime)
            {
                throw new InvalidOperationException(
                    "End time must be after start time.");
            }

            if (!await _repo
                .VenueExistsAsync(
                    dto.VenueId))
            {
                throw new InvalidOperationException(
                    "Venue not found.");
            }

            if (!await _repo
                .CategoryExistsAsync(
                    dto.CategoryId))
            {
                throw new InvalidOperationException(
                    "Category not found.");
            }

            var venue =
                await _repo.GetVenueAsync(
                    dto.VenueId);

            if (venue == null)
            {
                throw new InvalidOperationException(
                    "Venue not found.");
            }

            if (dto.Capacity >
                venue.Capacity)
            {
                throw new InvalidOperationException(
                    "Event capacity cannot exceed venue capacity.");
            }

            if (await _repo.HasOverlapAsync(
                dto.VenueId,
                dto.EventDate,
                dto.StartTime,
                dto.EndTime,
                excludeEventId))
            {
                throw new InvalidOperationException(
                    "Venue is already booked for an overlapping time.");
            }
        }

        private static EventDto Map(
            EventParkingReservation.Models.Event
                entity)
        {
            return new EventDto
            {
                EventId =
                    entity.EventId,

                EventName =
                    entity.EventName,

                EventDate =
                    entity.EventDate,

                StartTime =
                    entity.StartTime,

                EndTime =
                    entity.EndTime,

                Description =
                    entity.Description,

                TicketPrice =
                    entity.TicketPrice,

                ParkingFee =
                    entity.ParkingFee,

                Capacity =
                    entity.Capacity,

                VenueId =
                    entity.VenueId,

                VenueName =
                    entity.Venue?.VenueName
                    ?? string.Empty,

                CategoryId =
                    entity.CategoryId,

                CategoryName =
                    entity.Category
                        ?.CategoryName
                    ?? string.Empty
            };
        }
    }
}