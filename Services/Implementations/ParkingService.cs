using EventParkingReservation.DTOs.Parking;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class ParkingService : IParkingService
    {
        private readonly IParkingRepository _repo;

        public ParkingService(
            IParkingRepository repo)
        {
            _repo = repo;
        }

        // =====================================================
        // GET PARKING SLOTS BY EVENT
        // Admin + Customer can view
        // =====================================================

        public async Task<List<ParkingSlotDto>>
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

            var slots =
                await _repo.GetByEventIdAsync(
                    eventId);

            return slots
                .Select(Map)
                .ToList();
        }

        // =====================================================
        // CREATE PARKING SLOT - ADMIN
        // =====================================================

        public async Task<ParkingSlotDto> CreateAsync(
            int eventId,
            CreateParkingSlotDto dto)
        {
            var eventItem =
                await _repo.GetEventAsync(eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }

            var slotNumber =
                dto.SlotNumber
                    .Trim()
                    .ToUpperInvariant();

            var exists =
                await _repo.NumberExistsAsync(
                    eventId,
                    slotNumber);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Parking slot number already exists for this event.");
            }

            var slot =
                new ParkingSlot
                {
                    EventId = eventId,

                    SlotNumber =
                        slotNumber,

                    VehicleType =
                        dto.VehicleType.Trim(),

                    Fee =
                        dto.Fee,

                    Status =
                        "Available"
                };

            await _repo.AddAsync(slot);

            return Map(slot);
        }

        // =====================================================
        // UPDATE PARKING SLOT - ADMIN
        // =====================================================

        public async Task<ParkingSlotDto> UpdateAsync(
            int eventId,
            int slotId,
            UpdateParkingSlotDto dto)
        {
            var slot =
                await _repo.GetByIdAsync(slotId)
                ?? throw new KeyNotFoundException(
                    "Parking slot not found.");

            if (slot.EventId != eventId)
            {
                throw new KeyNotFoundException(
                    "Parking slot not found for this event.");
            }

            var hasReservation =
                await _repo.HasActiveReservationAsync(
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
                await _repo.NumberExistsAsync(
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
                dto.VehicleType.Trim();

            slot.Fee =
                dto.Fee;

            await _repo.UpdateAsync(slot);

            return Map(slot);
        }

        // =====================================================
        // DELETE PARKING SLOT - ADMIN
        // =====================================================

        public async Task DeleteAsync(
            int eventId,
            int slotId)
        {
            var slot =
                await _repo.GetByIdAsync(slotId)
                ?? throw new KeyNotFoundException(
                    "Parking slot not found.");

            if (slot.EventId != eventId)
            {
                throw new KeyNotFoundException(
                    "Parking slot not found for this event.");
            }

            var hasReservation =
                await _repo.HasActiveReservationAsync(
                    slotId);

            if (hasReservation)
            {
                throw new InvalidOperationException(
                    "Reserved parking slot cannot be deleted.");
            }

            await _repo.DeleteAsync(slot);
        }

        // =====================================================
        // MAPPER
        // =====================================================

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