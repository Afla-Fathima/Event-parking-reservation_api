using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventCategory>> GetAllAsync()
        {
            return await _context.EventCategories
                .AsNoTracking()
                .OrderBy(x => x.CategoryName)
                .ToListAsync();
        }

        public async Task<EventCategory?> GetByIdAsync(int id)
        {
            return await _context.EventCategories
                .FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task<bool> NameExistsAsync(
            string name,
            int? excludeId = null)
        {
            return await _context.EventCategories.AnyAsync(x =>
                x.CategoryName == name &&
                (!excludeId.HasValue ||
                 x.CategoryId != excludeId.Value));
        }

        public async Task AddAsync(EventCategory category)
        {
            _context.EventCategories.Add(category);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EventCategory category)
        {
            _context.EventCategories.Update(category);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(EventCategory category)
        {
            _context.EventCategories.Remove(category);

            await _context.SaveChangesAsync();
        }
    }
}
