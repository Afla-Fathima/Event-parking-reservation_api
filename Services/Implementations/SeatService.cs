using EventParkingReservation.DTOs.Seat;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _repo;

        public SeatService(
            ISeatRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SeatDto>>
            GetByEventIdAsync(
                int eventId)
        {
            var eventItem =
                await _repo.GetEventAsync(eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }

            var seats =
                await _repo.GetByEventIdAsync(
                    eventId);

            return seats
                .Select(Map)
                .ToList();
        }

        public async Task<SeatDto> CreateAsync(
            int eventId,
            CreateSeatDto dto)
        {
            var eventItem =
                await _repo.GetEventAsync(eventId)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            var seatCount =
                await _repo.CountByEventAsync(
                    eventId);

            if (seatCount >= eventItem.Capacity)
            {
                throw new InvalidOperationException(
                    "Seat count cannot exceed event capacity.");
            }

            var seatNumber =
                dto.SeatNumber
                    .Trim()
                    .ToUpperInvariant();

            var exists =
                await _repo.NumberExistsAsync(
                    eventId,
                    seatNumber);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Seat number already exists for this event.");
            }

            var seat =
                new Seat
                {
                    EventId =
                        eventId,

                    SeatNumber =
                        seatNumber,

                    SeatType =
                        dto.SeatType.Trim(),

                    Price =
                        dto.Price,

                    Status =
                        "Available"
                };

            await _repo.AddAsync(seat);

            return Map(seat);
        }

        public async Task<SeatDto> UpdateAsync(
            int eventId,
            int seatId,
            UpdateSeatDto dto)
        {
            var seat =
                await _repo.GetByIdAsync(seatId)
                ?? throw new KeyNotFoundException(
                    "Seat not found.");

            if (seat.EventId != eventId)
            {
                throw new KeyNotFoundException(
                    "Seat not found for this event.");
            }

            var booked =
                await _repo.HasActiveBookingAsync(
                    seatId);

            if (booked)
            {
                throw new InvalidOperationException(
                    "Booked seat cannot be changed.");
            }

            var seatNumber =
                dto.SeatNumber
                    .Trim()
                    .ToUpperInvariant();

            var exists =
                await _repo.NumberExistsAsync(
                    eventId,
                    seatNumber,
                    seatId);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Seat number already exists for this event.");
            }

            seat.SeatNumber =
                seatNumber;

            seat.SeatType =
                dto.SeatType.Trim();

            seat.Price =
                dto.Price;

            await _repo.UpdateAsync(seat);

            return Map(seat);
        }

        public async Task DeleteAsync(
            int eventId,
            int seatId)
        {
            var seat =
                await _repo.GetByIdAsync(seatId)
                ?? throw new KeyNotFoundException(
                    "Seat not found.");

            if (seat.EventId != eventId)
            {
                throw new KeyNotFoundException(
                    "Seat not found for this event.");
            }

            var booked =
                await _repo.HasActiveBookingAsync(
                    seatId);

            if (booked)
            {
                throw new InvalidOperationException(
                    "Booked seat cannot be deleted.");
            }

            await _repo.DeleteAsync(seat);
        }

        private static SeatDto Map(
            Seat seat)
        {
            return new SeatDto
            {
                SeatId =
                    seat.SeatId,

                EventId =
                    seat.EventId,

                SeatNumber =
                    seat.SeatNumber,

                SeatType =
                    seat.SeatType,

                Price =
                    seat.Price,

                Status =
                    seat.Status
            };
        }
    }
}