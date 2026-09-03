using EventParkingReservation.DTOs.Seat;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Implementations;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IEventRepository _eventRepository;

        public SeatService(
            ISeatRepository seatRepository,
            IEventRepository eventRepository)
        {
            _seatRepository = seatRepository;
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<SeatDto>>
            GetByEventAsync(int eventId)
        {
            var eventEntity =
                await _eventRepository.GetByIdAsync(
                    eventId)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            var seats =
                await _seatRepository
                    .GetByEventIdAsync(eventId);

            return seats.Select(Map);
        }

        public async Task<SeatDto> CreateAsync(
            int eventId,
            CreateSeatDto dto)
        {
            var eventEntity =
                await _eventRepository.GetByIdAsync(
                    eventId)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            var existingSeats =
                await _seatRepository
                    .GetByEventIdAsync(eventId);

            if (existingSeats.Count() >=
                eventEntity.Capacity)
            {
                throw new InvalidOperationException(
                    "Seat count cannot exceed event capacity.");
            }

            if (await _seatRepository
                .SeatNumberExistsAsync(
                    eventId,
                    dto.SeatNumber.Trim()))
            {
                throw new InvalidOperationException(
                    "Seat number already exists.");
            }

            var seat = new Seat
            {
                EventId = eventId,
                SeatNumber =
                    dto.SeatNumber.Trim(),
                SeatType =
                    dto.SeatType.Trim(),
                Price = dto.Price,
                Status = "Available"
            };

            await _seatRepository.AddAsync(seat);

            return Map(seat);
        }

        public async Task<SeatDto> UpdateAsync(
            int eventId,
            int seatId,
            UpdateSeatDto dto)
        {
            var seat =
                await _seatRepository.GetByIdAsync(
                    seatId)
                ?? throw new KeyNotFoundException(
                    "Seat not found.");

            if (seat.EventId != eventId)
            {
                throw new KeyNotFoundException(
                    "Seat does not belong to this event.");
            }

            if (await _seatRepository
                .HasActiveBookingAsync(seatId))
            {
                throw new InvalidOperationException(
                    "Booked seat cannot be modified.");
            }

            if (await _seatRepository
                .SeatNumberExistsAsync(
                    eventId,
                    dto.SeatNumber.Trim(),
                    seatId))
            {
                throw new InvalidOperationException(
                    "Seat number already exists.");
            }

            seat.SeatNumber =
                dto.SeatNumber.Trim();

            seat.SeatType =
                dto.SeatType.Trim();

            seat.Price =
                dto.Price;

            await _seatRepository.UpdateAsync(
                seat);

            return Map(seat);
        }

        public async Task DeleteAsync(
            int eventId,
            int seatId)
        {
            var seat =
                await _seatRepository.GetByIdAsync(
                    seatId)
                ?? throw new KeyNotFoundException(
                    "Seat not found.");

            if (seat.EventId != eventId)
            {
                throw new KeyNotFoundException(
                    "Seat does not belong to this event.");
            }

            if (await _seatRepository
                .HasActiveBookingAsync(seatId))
            {
                throw new InvalidOperationException(
                    "Booked seat cannot be deleted.");
            }

            await _seatRepository.DeleteAsync(
                seat);
        }

        private static SeatDto Map(Seat seat)
        {
            return new SeatDto
            {
                SeatId = seat.SeatId,
                EventId = seat.EventId,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType,
                Price = seat.Price,
                Status = seat.Status
            };
        }
    }
}
