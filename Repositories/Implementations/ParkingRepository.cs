using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class ParkingRepository
        : IParkingRepository
    {
        private readonly ApplicationDbContext
            _db;

        public ParkingRepository(
            ApplicationDbContext db)
        {
            _db = db;
        }


        // ===========================================
        // GET ALL PARKING SLOTS BY EVENT
        // ===========================================

        public Task<List<ParkingSlot>>
            GetByEventIdAsync(
                int eventId)
        {
            return _db.ParkingSlots
                .AsNoTracking()
                .Where(
                    x =>
                        x.EventId ==
                        eventId)
                .OrderBy(
                    x =>
                        x.SlotNumber)
                .ToListAsync();
        }


        // ===========================================
        // GET ONE
        // ===========================================

        public Task<ParkingSlot?>
            GetByIdAsync(
                int id)
        {
            return _db.ParkingSlots
                .FirstOrDefaultAsync(
                    x =>
                        x.ParkingSlotId ==
                        id);
        }


        // ===========================================
        // EVENT
        // ===========================================

        public Task<Event?>
            GetEventAsync(
                int eventId)
        {
            return _db.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.EventId ==
                        eventId);
        }


        // ===========================================
        // DUPLICATE SLOT CHECK
        // ===========================================

        public Task<bool>
            NumberExistsAsync(
                int eventId,
                string number,
                int? excludeId = null)
        {
            return _db.ParkingSlots
                .AnyAsync(
                    x =>
                        x.EventId ==
                            eventId &&

                        x.SlotNumber ==
                            number &&

                        (
                            !excludeId.HasValue ||

                            x.ParkingSlotId !=
                                excludeId.Value
                        ));
        }


        // ===========================================
        // ACTIVE RESERVATION CHECK
        // ===========================================

        public Task<bool>
            HasActiveReservationAsync(
                int slotId)
        {
            return _db
                .ParkingReservations
                .AnyAsync(
                    x =>
                        x.ParkingSlotId ==
                            slotId &&

                        x.Status ==
                            "Active");
        }


        // ===========================================
        // ADD ONE
        // ===========================================

        public async Task AddAsync(
            ParkingSlot slot)
        {
            _db.ParkingSlots.Add(
                slot);

            await _db
                .SaveChangesAsync();
        }


        // ===========================================
        // ADD MANY
        // ===========================================

        public async Task AddRangeAsync(
            IEnumerable<ParkingSlot> slots)
        {
            await _db.ParkingSlots
                .AddRangeAsync(
                    slots);

            await _db
                .SaveChangesAsync();
        }


        // ===========================================
        // UPDATE
        // ===========================================

        public async Task UpdateAsync(
            ParkingSlot slot)
        {
            _db.ParkingSlots.Update(
                slot);

            await _db
                .SaveChangesAsync();
        }


        // ===========================================
        // DELETE
        // ===========================================

        public async Task DeleteAsync(
            ParkingSlot slot)
        {
            _db.ParkingSlots.Remove(
                slot);

            await _db
                .SaveChangesAsync();
        }
    }
}