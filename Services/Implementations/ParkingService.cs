using EventParkingReservation.DTOs.Parking;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Implementations;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class ParkingService :
        IParkingService
    {
        private readonly IParkingRepository
            _parkingRepository;

        private readonly IEventRepository
            _eventRepository;

        public ParkingService(
            IParkingRepository parkingRepository,
            IEventRepository eventRepository)
        {
            _parkingRepository =
                parkingRepository;

            _eventRepository =
                eventRepository;
        }

        public async Task<IEnumerable<ParkingSlotDto>>
            GetByEventAsync(int eventId)
        {
            _ =
                await _eventRepository
                    .GetByIdAsync(eventId)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            var slots =
                await _parkingRepository
                    .GetByEventIdAsync(eventId);

            return slots.Select(Map);
        }

        public async Task<ParkingSlotDto>
            CreateAsync(
                int eventId,
                CreateParkingSlotDto dto)
        {
            _ =
                await _eventRepository
                    .GetByIdAsync(eventId)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            if (await _parkingRepository
                .SlotNumberExistsAsync(
                    eventId,
                    dto.SlotNumber.Trim()))
            {
                throw new InvalidOperationException(
                    "Parking slot number already exists.");
            }

            var slot = new ParkingSlot
            {
                EventId = eventId,

                SlotNumber =
                    dto.SlotNumber.Trim(),

                VehicleType =
                    dto.VehicleType.Trim(),

                Fee =
                    dto.Fee,

                Status =
                    "Available"
            };

            await _parkingRepository
                .AddAsync(slot);

            return Map(slot);
        }

        public async Task<ParkingSlotDto>
            UpdateAsync(
                int eventId,
                int slotId,
                UpdateParkingSlotDto dto)
        {
            var slot =
                await _parkingRepository
                    .GetByIdAsync(slotId)
                ?? throw new KeyNotFoundException(
                    "Parking slot not found.");

            if (slot.EventId != eventId)
            {
                throw new KeyNotFoundException(
                    "Parking slot does not belong to this event.");
            }

            if (await _parkingRepository
                .HasActiveReservationAsync(slotId))
            {
                throw new InvalidOperationException(
                    "Reserved parking slot cannot be modified.");
            }

            if (await _parkingRepository
                .SlotNumberExistsAsync(
                    eventId,
                    dto.SlotNumber.Trim(),
                    slotId))
            {
                throw new InvalidOperationException(
                    "Parking slot number already exists.");
            }

            slot.SlotNumber =
                dto.SlotNumber.Trim();

            slot.VehicleType =
                dto.VehicleType.Trim();

            slot.Fee =
                dto.Fee;

            await _parkingRepository
                .UpdateAsync(slot);

            return Map(slot);
        }

        public async Task DeleteAsync(
            int eventId,
            int slotId)
        {
            var slot =
                await _parkingRepository
                    .GetByIdAsync(slotId)
                ?? throw new KeyNotFoundException(
                    "Parking slot not found.");

            if (slot.EventId != eventId)
            {
                throw new KeyNotFoundException(
                    "Parking slot does not belong to this event.");
            }

            if (await _parkingRepository
                .HasActiveReservationAsync(slotId))
            {
                throw new InvalidOperationException(
                    "Reserved parking slot cannot be deleted.");
            }

            await _parkingRepository
                .DeleteAsync(slot);
        }

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
