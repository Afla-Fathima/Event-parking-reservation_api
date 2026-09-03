
using EventParkingReservation.DTOs.Seat;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private const int DefaultSeatCount = 50;
        private const int SeatsPerRow = 10;

        private readonly ISeatRepository _repository;


        public SeatService(
            ISeatRepository repository)
        {
            _repository = repository;
        }


        // ==========================================
        // GET EVENT SEATS
        // Automatically fills up to 50 seats
        // ==========================================

        public async Task<IEnumerable<SeatResponseDto>>
            GetByEventIdAsync(int eventId)
        {
            await EnsureDefaultSeatsAsync(
                eventId);

            var seats =
                await _repository
                    .GetByEventIdAsync(
                        eventId);

            return seats.Select(
                MapToResponse);
        }


        // ==========================================
        // AUTO GENERATE 50 SEATS
        // A01-A10
        // B01-B10
        // C01-C10
        // D01-D10
        // E01-E10
        // ==========================================

        private async Task
            EnsureDefaultSeatsAsync(
                int eventId)
        {
            var eventData =
                await _repository
                    .GetEventByIdAsync(
                        eventId);

            if (eventData == null)
            {
                throw new InvalidOperationException(
                    "Event not found");
            }


            var existingSeats =
                (
                    await _repository
                        .GetByEventIdAsync(
                            eventId)
                )
                .ToList();


            // Never exceed event capacity.
            var requiredSeatCount =
                Math.Min(
                    DefaultSeatCount,
                    eventData.Capacity);


            if (
                existingSeats.Count
                >= requiredSeatCount)
            {
                return;
            }


            var existingNumbers =
                existingSeats
                    .Select(s =>
                        s.SeatNumber
                            .Trim()
                            .ToUpperInvariant())
                    .ToHashSet();


            // If VIP seat was already manually created
            // use that price for generated VIP seats.
            var existingVipPrice =
                existingSeats
                    .FirstOrDefault(s =>
                        s.SeatType.Equals(
                            "VIP",
                            StringComparison.OrdinalIgnoreCase))
                    ?.Price;


            var vipPrice =
                existingVipPrice
                ?? eventData.TicketPrice;


            var regularPrice =
                eventData.TicketPrice;


            var newSeats =
                new List<Seat>();


            for (
                int index = 0;
                index < DefaultSeatCount;
                index++)
            {
                if (
                    existingSeats.Count
                    + newSeats.Count
                    >= requiredSeatCount)
                {
                    break;
                }


                int rowIndex =
                    index / SeatsPerRow;

                int position =
                    (index % SeatsPerRow) + 1;


                char rowLetter =
                    (char)(
                        'A' + rowIndex
                    );


                string seatNumber =
                    $"{rowLetter}{position:00}";


                if (
                    existingNumbers.Contains(
                        seatNumber))
                {
                    continue;
                }


                bool isVip =
                    rowIndex == 0;


                var seat =
                    new Seat
                    {
                        EventId =
                            eventId,

                        SeatNumber =
                            seatNumber,

                        SeatType =
                            isVip
                                ? "VIP"
                                : "Regular",

                        Price =
                            isVip
                                ? vipPrice
                                : regularPrice,

                        Status =
                            "Available"
                    };


                newSeats.Add(
                    seat);

                existingNumbers.Add(
                    seatNumber);
            }


            await _repository
                .AddRangeAsync(
                    newSeats);
        }


        // ==========================================
        // GET SEAT
        // ==========================================

        public async Task<SeatResponseDto?>
            GetByIdAsync(int seatId)
        {
            var seat =
                await _repository
                    .GetByIdAsync(
                        seatId);

            if (seat == null)
            {
                return null;
            }

            return MapToResponse(
                seat);
        }


        // ==========================================
        // MANUAL CREATE
        // ==========================================

        public async Task<SeatResponseDto>
            AddAsync(CreateSeatDto dto)
        {
            var eventData =
                await _repository
                    .GetEventByIdAsync(
                        dto.EventId);

            if (eventData == null)
            {
                throw new InvalidOperationException(
                    "Event not found");
            }


            bool duplicate =
                await _repository
                    .SeatNumberExistsAsync(
                        dto.EventId,
                        dto.SeatNumber.Trim());

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "Seat number already exists for this event");
            }


            int currentSeatCount =
                await _repository
                    .CountByEventAsync(
                        dto.EventId);


            if (
                currentSeatCount
                >= eventData.Capacity)
            {
                throw new InvalidOperationException(
                    "Event seat capacity has been reached");
            }


            var seat =
                new Seat
                {
                    EventId =
                        dto.EventId,

                    SeatNumber =
                        dto.SeatNumber.Trim(),

                    SeatType =
                        dto.SeatType.Trim(),

                    Price =
                        dto.Price,

                    Status =
                        "Available"
                };


            await _repository
                .AddAsync(
                    seat);


            return MapToResponse(
                seat);
        }


        // ==========================================
        // UPDATE
        // ==========================================

        public async Task<SeatResponseDto>
            UpdateAsync(
                int seatId,
                UpdateSeatDto dto)
        {
            var seat =
                await _repository
                    .GetByIdAsync(
                        seatId);

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat not found");
            }


            bool hasBooking =
                await _repository
                    .HasBookingsAsync(
                        seatId);

            if (hasBooking)
            {
                throw new InvalidOperationException(
                    "Booked seat cannot be modified");
            }


            bool duplicate =
                await _repository
                    .SeatNumberExistsAsync(
                        seat.EventId,
                        dto.SeatNumber.Trim(),
                        seatId);


            if (duplicate)
            {
                throw new InvalidOperationException(
                    "Seat number already exists for this event");
            }


            seat.SeatNumber =
                dto.SeatNumber.Trim();

            seat.SeatType =
                dto.SeatType.Trim();

            seat.Price =
                dto.Price;


            await _repository
                .UpdateAsync(
                    seat);


            return MapToResponse(
                seat);
        }


        // ==========================================
        // DELETE
        // ==========================================

        public async Task
            DeleteAsync(int seatId)
        {
            var seat =
                await _repository
                    .GetByIdAsync(
                        seatId);

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat not found");
            }


            bool hasBooking =
                await _repository
                    .HasBookingsAsync(
                        seatId);

            if (hasBooking)
            {
                throw new InvalidOperationException(
                    "Booked seat cannot be deleted");
            }


            await _repository
                .DeleteAsync(
                    seat);
        }


        private static SeatResponseDto
            MapToResponse(
                Seat seat)
        {
            return new SeatResponseDto
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