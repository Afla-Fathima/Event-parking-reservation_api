using EventParkingReservation.DTOs.Venue;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _repository;

        public VenueService(
            IVenueRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VenueDto>>
            GetAllAsync()
        {
            return (await _repository.GetAllAsync())
                .Select(Map);
        }

        public async Task<VenueDto>
            GetByIdAsync(int id)
        {
            Venue venue =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Venue not found.");

            return Map(venue);
        }

        public async Task<VenueDto>
            CreateAsync(CreateVenueDto dto)
        {
            Venue venue = new()
            {
                VenueName =
                    dto.VenueName.Trim(),

                Location =
                    dto.Location.Trim(),

                Capacity =
                    dto.Capacity,

                Description =
                    dto.Description
            };

            await _repository.AddAsync(venue);

            return Map(venue);
        }

        public async Task<VenueDto>
            UpdateAsync(
                int id,
                UpdateVenueDto dto)
        {
            Venue venue =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Venue not found.");

            venue.VenueName =
                dto.VenueName.Trim();

            venue.Location =
                dto.Location.Trim();

            venue.Capacity =
                dto.Capacity;

            venue.Description =
                dto.Description;

            await _repository
                .UpdateAsync(venue);

            return Map(venue);
        }

        public async Task DeleteAsync(int id)
        {
            Venue venue =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Venue not found.");

            if (await _repository
                .HasUpcomingEventsAsync(id))
            {
                throw new InvalidOperationException(
                    "Venue has upcoming events and cannot be deleted.");
            }

            await _repository.DeleteAsync(venue);
        }

        private static VenueDto Map(
            Venue venue)
        {
            return new VenueDto
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                Location = venue.Location,
                Capacity = venue.Capacity,
                Description = venue.Description
            };
        }
    }
}
