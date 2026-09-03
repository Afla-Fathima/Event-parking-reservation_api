using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class ParkingRepository : IParkingRepository
    {
        private readonly ApplicationDbContext _context;

        public ParkingRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParkingSlot>>
            GetByEventIdAsync(int eventId)
        {
            return await _context.ParkingSlots
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.SlotNumber)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ParkingSlot?> GetByIdAsync(
            int parkingSlotId)
        {
            return await _context.ParkingSlots
                .FirstOrDefaultAsync(x =>
                    x.ParkingSlotId == parkingSlotId);
        }

        public async Task<bool> SlotNumberExistsAsync(
            int eventId,
            string slotNumber,
            int? excludeId = null)
        {
            return await _context.ParkingSlots.AnyAsync(x =>
                x.EventId == eventId &&
                x.SlotNumber == slotNumber &&
                (!excludeId.HasValue ||
                 x.ParkingSlotId != excludeId.Value));
        }

        public async Task<bool> HasActiveReservationAsync(
            int parkingSlotId)
        {
            return await _context.ParkingReservations
                .AnyAsync(x =>
                    x.ParkingSlotId == parkingSlotId &&
                    x.Status == "Active");
        }

        public async Task AddAsync(ParkingSlot slot)
        {
            _context.ParkingSlots.Add(slot);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ParkingSlot slot)
        {
            _context.ParkingSlots.Update(slot);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ParkingSlot slot)
        {
            _context.ParkingSlots.Remove(slot);

            await _context.SaveChangesAsync();
        }
    }
}
