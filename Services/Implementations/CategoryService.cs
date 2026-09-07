using EventParkingReservation.DTOs.Category;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class CategoryService :
        ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(
            ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoryDto>>
            GetAllAsync()
        {
            return (await _repository.GetAllAsync())
                .Select(Map);
        }

        public async Task<CategoryDto>
            GetByIdAsync(int id)
        {
            EventCategory category =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Category not found.");

            return Map(category);
        }

        public async Task<CategoryDto>
            CreateAsync(CreateCategoryDto dto)
        {
            if (await _repository.NameExistsAsync(
                dto.CategoryName.Trim()))
            {
                throw new InvalidOperationException(
                    "Category name already exists.");
            }

            EventCategory category = new()
            {
                CategoryName =
                    dto.CategoryName.Trim(),

                Description =
                    dto.Description,

                Status =
                    "Active"
            };

            await _repository.AddAsync(category);

            return Map(category);
        }

        public async Task<CategoryDto>
            UpdateAsync(
                int id,
                UpdateCategoryDto dto)
        {
            EventCategory category =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Category not found.");

            if (await _repository.NameExistsAsync(
                dto.CategoryName.Trim(),
                id))
            {
                throw new InvalidOperationException(
                    "Category name already exists.");
            }

            category.CategoryName =
                dto.CategoryName.Trim();

            category.Description =
                dto.Description;

            category.Status =
                dto.Status;

            await _repository.UpdateAsync(category);

            return Map(category);
        }

        public async Task DeleteAsync(int id)
        {
            EventCategory category =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Category not found.");

            await _repository.DeleteAsync(category);
        }

        private static CategoryDto Map(
            EventCategory category)
        {
            return new CategoryDto
            {
                CategoryId =
                    category.CategoryId,

                CategoryName =
                    category.CategoryName,

                Description =
                    category.Description,

                Status =
                    category.Status
            };
        }
    }
}
