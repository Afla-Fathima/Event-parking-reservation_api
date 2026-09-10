using EventParkingReservation.DTOs.Parking;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class ParkingService
        : IParkingService
    {
        private readonly IParkingRepository
            _repo;

        private const int
            DefaultParkingCount = 20;


        public ParkingService(
            IParkingRepository repo)
        {
            _repo = repo;
        }


        // ============================================
        // GET PARKING MAP
        // ============================================

        public async Task<List<ParkingSlotDto>>
            GetByEventIdAsync(
                int eventId)
        {
            var eventItem =
                await _repo
                    .GetEventAsync(
                        eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }

            var slots =
                await _repo
                    .GetByEventIdAsync(
                        eventId);

            return slots
                .OrderBy(
                    x =>
                        x.SlotNumber)
                .Select(Map)
                .ToList();
        }


        // ============================================
        // CREATE ONE SLOT
        // ============================================

        public async Task<ParkingSlotDto>
            CreateAsync(
                int eventId,
                CreateParkingSlotDto dto)
        {
            var eventItem =
                await _repo
                    .GetEventAsync(
                        eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }

            if (
                string.IsNullOrWhiteSpace(
                    dto.SlotNumber))
            {
                throw new InvalidOperationException(
                    "Parking slot number is required.");
            }

            var slotNumber =
                dto.SlotNumber
                    .Trim()
                    .ToUpperInvariant();

            var exists =
                await _repo
                    .NumberExistsAsync(
                        eventId,
                        slotNumber);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Parking slot {slotNumber} already exists.");
            }

            var currentSlots =
                await _repo
                    .GetByEventIdAsync(
                        eventId);

            // Our project allows exactly 20
            if (
                currentSlots.Count >=
                DefaultParkingCount)
            {
                throw new InvalidOperationException(
                    "This event already has 20 parking slots.");
            }

            var slot =
                new ParkingSlot
                {
                    EventId =
                        eventId,

                    SlotNumber =
                        slotNumber,

                    VehicleType =
                        string.IsNullOrWhiteSpace(
                            dto.VehicleType)
                            ? "Car"
                            : dto.VehicleType
                                .Trim(),

                    Fee =
                        dto.Fee,

                    Status =
                        "Available"
                };

            await _repo
                .AddAsync(
                    slot);

            return Map(
                slot);
        }


        // ============================================
        // GENERATE PARKING SLOTS
        //
        // IMPORTANT:
        // Existing slots are NOT deleted.
        //
        // Example:
        // existing B01 = 1
        // generator creates 19 new slots
        // FINAL TOTAL = 20
        // ============================================

        public async Task<int>
            GenerateDefaultSlotsAsync(
                int eventId)
        {
            var eventItem =
                await _repo
                    .GetEventAsync(
                        eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }


            var currentSlots =
                await _repo
                    .GetByEventIdAsync(
                        eventId);


            // Already complete
            if (
                currentSlots.Count >=
                DefaultParkingCount)
            {
                throw new InvalidOperationException(
                    "Parking layout already contains 20 slots.");
            }


            var existingNumbers =
                currentSlots
                    .Select(
                        x =>
                            x.SlotNumber
                                .Trim()
                                .ToUpperInvariant())
                    .ToHashSet(
                        StringComparer
                            .OrdinalIgnoreCase);


            var slotsNeeded =
                DefaultParkingCount -
                currentSlots.Count;


            var newSlots =
                new List<ParkingSlot>();


            // Generate P01 - P20
            for (
                int number = 1;
                number <= 20;
                number++)
            {
                if (
                    newSlots.Count >=
                    slotsNeeded)
                {
                    break;
                }


                var slotNumber =
                    $"P{number:00}";


                if (
                    existingNumbers.Contains(
                        slotNumber))
                {
                    continue;
                }


                var slot =
                    new ParkingSlot
                    {
                        EventId =
                            eventId,

                        SlotNumber =
                            slotNumber,

                        VehicleType =
                            "Car",

                        // BRD:
                        // parking fee belongs to event/layout
                        Fee =
                            eventItem
                                .ParkingFee,

                        Status =
                            "Available"
                    };


                newSlots.Add(
                    slot);


                existingNumbers.Add(
                    slotNumber);
            }


            if (
                newSlots.Count == 0)
            {
                throw new InvalidOperationException(
                    "No parking slots need to be generated.");
            }


            await _repo
                .AddRangeAsync(
                    newSlots);


            return newSlots.Count;
        }


        // ============================================
        // UPDATE SLOT
        // ============================================

        public async Task<ParkingSlotDto>
            UpdateAsync(
                int eventId,
                int slotId,
                UpdateParkingSlotDto dto)
        {
            var slot =
                await _repo
                    .GetByIdAsync(
                        slotId)
                ?? throw new KeyNotFoundException(
                    "Parking slot not found.");


            if (
                slot.EventId !=
                eventId)
            {
                throw new KeyNotFoundException(
                    "Parking slot not found for this event.");
            }


            var hasReservation =
                await _repo
                    .HasActiveReservationAsync(
                        slotId);


            if (hasReservation)
            {
                throw new InvalidOperationException(
                    "Reserved parking slot cannot be changed.");
            }


            var slotNumber =
                dto.SlotNumber
                    .Trim()
                    .ToUpperInvariant();


            var exists =
                await _repo
                    .NumberExistsAsync(
                        eventId,
                        slotNumber,
                        slotId);


            if (exists)
            {
                throw new InvalidOperationException(
                    "Parking slot number already exists for this event.");
            }


            slot.SlotNumber =
                slotNumber;


            slot.VehicleType =
                string.IsNullOrWhiteSpace(
                    dto.VehicleType)
                    ? "Car"
                    : dto.VehicleType.Trim();


            slot.Fee =
                dto.Fee;


            await _repo
                .UpdateAsync(
                    slot);


            return Map(
                slot);
        }


        // ============================================
        // DELETE SLOT
        // ============================================

        public async Task DeleteAsync(
            int eventId,
            int slotId)
        {
            var slot =
                await _repo
                    .GetByIdAsync(
                        slotId)
                ?? throw new KeyNotFoundException(
                    "Parking slot not found.");


            if (
                slot.EventId !=
                eventId)
            {
                throw new KeyNotFoundException(
                    "Parking slot not found for this event.");
            }


            var hasReservation =
                await _repo
                    .HasActiveReservationAsync(
                        slotId);


            if (hasReservation)
            {
                throw new InvalidOperationException(
                    "Reserved parking slot cannot be deleted.");
            }


            await _repo
                .DeleteAsync(
                    slot);
        }


        // ============================================
        // MAP
        // ============================================

        private static ParkingSlotDto Map(
            ParkingSlot slot)
        {
            return new ParkingSlotDto
            {
                ParkingSlotId =
                    slot.ParkingSlotId,

                EventId =
                    slot.EventId,

                SlotNumber =
                    slot.SlotNumber,

                VehicleType =
                    slot.VehicleType,

                Fee =
                    slot.Fee,

                Status =
                    slot.Status
            };
        }
    }
}