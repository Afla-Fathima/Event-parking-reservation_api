using EventParkingReservation.DTOs.ParkingSlot;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class ParkingSlotService
        : IParkingSlotService
    {
        private const int DefaultParkingCount =
            20;


        private readonly IParkingSlotRepository
            _repository;


        public ParkingSlotService(
            IParkingSlotRepository repository)
        {
            _repository =
                repository;
        }


        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<
            IEnumerable<ParkingSlotResponseDto>>
            GetAllAsync()
        {
            var slots =
                await _repository
                    .GetAllAsync();


            return slots.Select(
                MapToResponse
            );
        }


        // ==========================================
        // GET PARKING BY EVENT
        // AUTO CREATES 20 PARKING SLOTS
        // ==========================================

        public async Task<
            IEnumerable<ParkingSlotResponseDto>>
            GetByEventIdAsync(
                int eventId)
        {
            await EnsureDefaultParkingAsync(
                eventId
            );


            var slots =
                await _repository
                    .GetByEventIdAsync(
                        eventId
                    );


            return slots.Select(
                MapToResponse
            );
        }


        // ==========================================
        // AUTO CREATE PARKING
        //
        // P01 - P15 = CAR
        // P16 - P20 = BIKE
        // ==========================================

        private async Task
            EnsureDefaultParkingAsync(
                int eventId)
        {
            var eventData =
                await _repository
                    .GetEventByIdAsync(
                        eventId
                    );


            if (eventData == null)
            {
                throw new InvalidOperationException(
                    "Event not found."
                );
            }


            var existingSlots =
                (
                    await _repository
                        .GetByEventIdAsync(
                            eventId
                        )
                )
                .ToList();


            if (
                existingSlots.Count
                >= DefaultParkingCount
            )
            {
                return;
            }


            var existingNumbers =
                existingSlots
                    .Select(
                        slot =>
                            slot.SlotNumber
                                .Trim()
                                .ToUpperInvariant()
                    )
                    .ToHashSet();


            var newSlots =
                new List<ParkingSlot>();


            for (
                int number = 1;
                number <= DefaultParkingCount;
                number++)
            {
                string slotNumber =
                    $"P{number:00}";


                if (
                    existingNumbers.Contains(
                        slotNumber
                    )
                )
                {
                    continue;
                }


                string vehicleType =
                    number <= 15
                        ? "Car"
                        : "Bike";


                var parkingSlot =
                    new ParkingSlot
                    {
                        EventId =
                            eventId,

                        SlotNumber =
                            slotNumber,

                        VehicleType =
                            vehicleType,

                        Fee =
                            eventData
                                .ParkingFee,

                        Status =
                            "Available"
                    };


                newSlots.Add(
                    parkingSlot
                );


                existingNumbers.Add(
                    slotNumber
                );


                if (
                    existingSlots.Count
                    + newSlots.Count
                    >= DefaultParkingCount
                )
                {
                    break;
                }
            }


            await _repository
                .AddRangeAsync(
                    newSlots
                );
        }


        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<
            ParkingSlotResponseDto?>
            GetByIdAsync(
                int id)
        {
            var slot =
                await _repository
                    .GetByIdAsync(
                        id
                    );


            if (slot == null)
            {
                return null;
            }


            return MapToResponse(
                slot
            );
        }


        // ==========================================
        // MANUAL CREATE
        // ==========================================

        public async Task<
            ParkingSlotResponseDto>
            AddAsync(
                CreateParkingSlotDto dto)
        {
            bool eventExists =
                await _repository
                    .EventExistsAsync(
                        dto.EventId
                    );


            if (!eventExists)
            {
                throw new InvalidOperationException(
                    "Event not found."
                );
            }


            bool duplicate =
                await _repository
                    .SlotNumberExistsAsync(
                        dto.EventId,
                        dto.SlotNumber
                            .Trim()
                    );


            if (duplicate)
            {
                throw new InvalidOperationException(
                    "Parking slot number already exists for this event."
                );
            }


            var slot =
                new ParkingSlot
                {
                    EventId =
                        dto.EventId,

                    SlotNumber =
                        dto.SlotNumber
                            .Trim(),

                    VehicleType =
                        dto.VehicleType
                            .Trim(),

                    Fee =
                        dto.Fee,

                    Status =
                        "Available"
                };


            await _repository
                .AddAsync(
                    slot
                );


            return MapToResponse(
                slot
            );
        }


        // ==========================================
        // UPDATE
        // ==========================================

        public async Task<
            ParkingSlotResponseDto>
            UpdateAsync(
                int id,
                UpdateParkingSlotDto dto)
        {
            var slot =
                await _repository
                    .GetByIdAsync(
                        id
                    );


            if (slot == null)
            {
                throw new KeyNotFoundException(
                    "Parking slot not found."
                );
            }


            bool hasReservation =
                await _repository
                    .HasReservationAsync(
                        id
                    );


            if (hasReservation)
            {
                throw new InvalidOperationException(
                    "Reserved parking slot cannot be modified."
                );
            }


            bool duplicate =
                await _repository
                    .SlotNumberExistsAsync(
                        slot.EventId,
                        dto.SlotNumber
                            .Trim(),
                        id
                    );


            if (duplicate)
            {
                throw new InvalidOperationException(
                    "Parking slot number already exists for this event."
                );
            }


            slot.SlotNumber =
                dto.SlotNumber
                    .Trim();


            slot.VehicleType =
                dto.VehicleType
                    .Trim();


            slot.Fee =
                dto.Fee;


            await _repository
                .UpdateAsync(
                    slot
                );


            return MapToResponse(
                slot
            );
        }


        // ==========================================
        // DELETE
        // ==========================================

        public async Task
            DeleteAsync(
                int id)
        {
            var slot =
                await _repository
                    .GetByIdAsync(
                        id
                    );


            if (slot == null)
            {
                throw new KeyNotFoundException(
                    "Parking slot not found."
                );
            }


            bool hasReservation =
                await _repository
                    .HasReservationAsync(
                        id
                    );


            if (hasReservation)
            {
                throw new InvalidOperationException(
                    "Reserved parking slot cannot be deleted."
                );
            }


            await _repository
                .DeleteAsync(
                    slot
                );
        }


        // ==========================================
        // MAP RESPONSE
        // ==========================================

        private static ParkingSlotResponseDto
            MapToResponse(
                ParkingSlot slot)
        {
            return new ParkingSlotResponseDto
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