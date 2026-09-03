using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class ParkingSlotRepository : IParkingSlotRepository
    {
        private readonly ApplicationDbContext _context;

        public ParkingSlotRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<IEnumerable<ParkingSlot>>
            GetAllAsync()
        {
            return await _context.ParkingSlots
                .AsNoTracking()
                .OrderBy(p => p.EventId)
                .ThenBy(p => p.SlotNumber)
                .ToListAsync();
        }


        // ==========================================
        // GET BY EVENT
        // ==========================================

        public async Task<IEnumerable<ParkingSlot>>
            GetByEventIdAsync(int eventId)
        {
            return await _context.ParkingSlots
                .AsNoTracking()
                .Where(p => p.EventId == eventId)
                .OrderBy(p => p.SlotNumber)
                .ToListAsync();
        }


        // ==========================================
        // ALTERNATIVE EVENT METHOD
        // ==========================================

        public async Task<IEnumerable<ParkingSlot>>
            GetEventParkingSlotsAsync(int eventId)
        {
            return await _context.ParkingSlots
                .AsNoTracking()
                .Where(p => p.EventId == eventId)
                .OrderBy(p => p.SlotNumber)
                .ToListAsync();
        }


        // ==========================================
        // GET SLOT BY ID
        // ==========================================

        public async Task<ParkingSlot?>
            GetByIdAsync(int id)
        {
            return await _context.ParkingSlots
                .FirstOrDefaultAsync(
                    p => p.ParkingSlotId == id
                );
        }


        // ==========================================
        // GET EVENT BY ID
        // ==========================================

        public async Task<Event?>
            GetEventByIdAsync(int eventId)
        {
            return await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e => e.EventId == eventId
                );
        }


        // ==========================================
        // EVENT EXISTS
        // ==========================================

        public async Task<bool>
            EventExistsAsync(int eventId)
        {
            return await _context.Events
                .AnyAsync(
                    e => e.EventId == eventId
                );
        }


        // ==========================================
        // COUNT EVENT PARKING
        // ==========================================

        public async Task<int>
            CountByEventAsync(int eventId)
        {
            return await _context.ParkingSlots
                .CountAsync(
                    p => p.EventId == eventId
                );
        }


        // ==========================================
        // SLOT NUMBER EXISTS
        // ==========================================

        public async Task<bool>
            SlotNumberExistsAsync(
                int eventId,
                string slotNumber,
                int? excludeId = null)
        {
            string normalizedSlotNumber =
                slotNumber.Trim();

            var query =
                _context.ParkingSlots
                    .Where(
                        p =>
                            p.EventId == eventId
                            &&
                            p.SlotNumber
                            == normalizedSlotNumber
                    );


            if (excludeId.HasValue)
            {
                query =
                    query.Where(
                        p =>
                            p.ParkingSlotId
                            != excludeId.Value
                    );
            }


            return await query.AnyAsync();
        }


        // ==========================================
        // RESERVATION EXISTS
        // ==========================================

        public async Task<bool>
            HasReservationAsync(int parkingSlotId)
        {
            return await _context.ParkingReservations
                .AnyAsync(
                    r =>
                        r.ParkingSlotId
                        == parkingSlotId
                );
        }


        // ==========================================
        // ADD ONE
        // ==========================================

        public async Task AddAsync(
            ParkingSlot parkingSlot)
        {
            await _context.ParkingSlots
                .AddAsync(parkingSlot);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // ADD MULTIPLE
        // ==========================================

        public async Task AddRangeAsync(
            IEnumerable<ParkingSlot> parkingSlots)
        {
            var slots =
                parkingSlots.ToList();

            if (slots.Count == 0)
            {
                return;
            }


            await _context.ParkingSlots
                .AddRangeAsync(slots);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // ALTERNATIVE ADD MULTIPLE METHOD
        // ==========================================

        public async Task AddParkingSlotsAsync(
            IEnumerable<ParkingSlot> parkingSlots)
        {
            var slots =
                parkingSlots.ToList();

            if (slots.Count == 0)
            {
                return;
            }


            await _context.ParkingSlots
                .AddRangeAsync(slots);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // UPDATE
        // ==========================================

        public async Task UpdateAsync(
            ParkingSlot parkingSlot)
        {
            _context.ParkingSlots
                .Update(parkingSlot);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // DELETE
        // ==========================================

        public async Task DeleteAsync(
            ParkingSlot parkingSlot)
        {
            _context.ParkingSlots
                .Remove(parkingSlot);

            await _context.SaveChangesAsync();
        }
    }
}