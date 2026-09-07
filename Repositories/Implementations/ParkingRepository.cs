using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class ParkingRepository : IParkingRepository
    {
        private readonly ApplicationDbContext _db;

        public ParkingRepository(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<List<ParkingSlot>>
            GetByEventIdAsync(
                int eventId)
        {
            return _db.ParkingSlots
                .AsNoTracking()
                .Where(x =>
                    x.EventId == eventId)
                .OrderBy(x =>
                    x.SlotNumber)
                .ToListAsync();
        }

        public Task<ParkingSlot?> GetByIdAsync(
            int id)
        {
            return _db.ParkingSlots
                .FirstOrDefaultAsync(x =>
                    x.ParkingSlotId == id);
        }

        public Task<Event?> GetEventAsync(
            int eventId)
        {
            return _db.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.EventId == eventId);
        }

        public Task<bool> NumberExistsAsync(
            int eventId,
            string number,
            int? excludeId = null)
        {
            return _db.ParkingSlots.AnyAsync(x =>
                x.EventId == eventId &&
                x.SlotNumber == number &&
                (
                    !excludeId.HasValue ||
                    x.ParkingSlotId !=
                        excludeId.Value
                ));
        }

        public Task<bool>
            HasActiveReservationAsync(
                int slotId)
        {
            return _db.ParkingReservations
                .AnyAsync(x =>
                    x.ParkingSlotId ==
                        slotId &&
                    x.Status ==
                        "Active");
        }

        public async Task AddAsync(
            ParkingSlot slot)
        {
            _db.ParkingSlots.Add(slot);

            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(
            ParkingSlot slot)
        {
            _db.ParkingSlots.Update(slot);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(
            ParkingSlot slot)
        {
            _db.ParkingSlots.Remove(slot);

            await _db.SaveChangesAsync();
        }
    }
}